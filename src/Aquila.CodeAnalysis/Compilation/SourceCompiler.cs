using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis.Passes;
using Aquila.CodeAnalysis.Lowering;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using SourceMethodSymbol = Aquila.CodeAnalysis.Symbols.SourceMethodSymbol;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// Performs compilation of all source methods.
    /// </summary>
    internal class SourceCompiler
    {
        readonly AquilaCompilation _compilation;
        readonly PEModuleBuilder _moduleBuilder;
        readonly bool _emittingPdb;
        readonly DiagnosticBag _diagnostics;
        readonly CancellationToken _cancellationToken;

        readonly Worklist<BoundBlock> _worklist;

        /// <summary>
        /// Number of control flow graph transformation cycles to do at most.
        /// <c>0</c> to disable the lowering.
        /// </summary>
        int MaxTransformCount => _compilation.Options.OptimizationLevel.GraphTransformationCount();

        public bool ConcurrentBuild => _compilation.Options.ConcurrentBuild;

        private SourceCompiler(AquilaCompilation compilation, PEModuleBuilder moduleBuilder, bool emittingPdb,
            DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            Contract.ThrowIfNull(compilation);
            Contract.ThrowIfNull(diagnostics);

            _compilation = compilation;
            _compilation.EnsureSourceAssembly();
            
            _moduleBuilder = moduleBuilder;
            _emittingPdb = emittingPdb;
            _diagnostics = diagnostics;
            _cancellationToken = cancellationToken;

            // parallel worklist algorithm
            _worklist = new Worklist<BoundBlock>(AnalyzeBlock);
        }

        void WalkSourceMethods(Action<SourceMethodSymbolBase> action, bool allowParallel = false)
        {
            var methods = _compilation.SourceSymbolCollection.GetSourceMethods();
            var viewMethods = _compilation.SourceSymbolCollection.GetViewTypes().SelectMany(x=>x.GetMembers().OfType<SourceViewTypeSymbol.MethodTreeBuilderSymbol>());

            methods = methods.Union(viewMethods);
            
            if (ConcurrentBuild && allowParallel)
            {
                Parallel.ForEach(methods, action);
            }
            else
            {
                methods.ForEach(action);
            }
        }

        void WalkSynthesizedMethods(Action<SynthesizedMethodSymbol> action, bool allowParallel = false)
        {
            var platformSynth = _compilation.PlatformSymbolCollection.SynthesizedTypes.SelectMany(x =>
                x.GetMembers().OfType<SynthesizedMethodSymbol>());

            var moduleTypes = _compilation.SourceSymbolCollection.GetModuleTypes().ToImmutableArray();

            var moduleMethods = moduleTypes.SelectMany(x => x.GetMembers().OfType<SynthesizedMethodSymbol>());
            var moduleNestedTypesMethods = moduleTypes.SelectMany(x =>
                x.GetTypeMembers().SelectMany(y => y.GetMembers().OfType<SynthesizedMethodSymbol>()));

            var viewMethods = _compilation.SourceSymbolCollection.GetViewTypes()
                .SelectMany(x => x.GetMembers().OfType<SynthesizedMethodSymbol>());
            
            var methods = platformSynth.Union(moduleMethods).Union(moduleNestedTypesMethods).Union(viewMethods);

            if (ConcurrentBuild && allowParallel)
            {
                Parallel.ForEach(methods, action);
            }
            else
            {
                methods.ForEach(action);
            }
        }

        void WalkTypes(Action<SourceModuleTypeSymbol> action, bool allowParallel = false)
        {
            var types = _compilation.SourceSymbolCollection.GetModuleTypes();
            
            if (ConcurrentBuild && allowParallel)
            {
                Parallel.ForEach(types, action);
            }
            else
            {
                types.ForEach(action);
            }
        }

        /// <summary>
        /// Enqueues method's start block for analysis.
        /// </summary>
        void EnqueueMethod(SourceMethodSymbolBase method)
        {
            Contract.ThrowIfNull(method);
            // lazily binds CFG and
            // adds their entry block to the worklist

            _worklist.Enqueue(method.ControlFlowGraph?.Start);
            

            // enqueue method parameter default values
            method.SourceParameters.ForEach(p =>
            {
                if (p.Initializer != null)
                {
                    EnqueueExpression(p.Initializer);
                }
            });
        }

        /// <summary>
        /// Enqueues the standalone expression for analysis.
        /// </summary>
        void EnqueueExpression(BoundExpression expression)
        {
            Contract.ThrowIfNull(expression);
            
            var dummy = new BoundBlock()
            {
                FlowState = new FlowState(new FlowContext(null)),
            };

            dummy.Add(new BoundExpressionStmt(expression));

            _worklist.Enqueue(dummy);
        }

        internal void ReanalyzeMethods()
        {
            this.WalkSourceMethods(method => _worklist.Enqueue(method.ControlFlowGraph.Start));
        }

        internal void AnalyzeMethods()
        {
            // analyse blocks
            _worklist.DoAll(concurrent: ConcurrentBuild);
        }

        void AnalyzeBlock(BoundBlock block)
        {
            block.Accept(AnalysisFactory());

            foreach (var lambda in block.FlowState.FlowContext.Lambdas)
            {
                EnqueueMethod(lambda);    
            }
        }

        GraphVisitor<VoidStruct> AnalysisFactory()
        {
            return new ExpressionAnalysis<VoidStruct>(_worklist, _compilation.GlobalSemantics);
        }

        #region Nested class: LateStaticCallsLookup

        /// <summary>
        /// Lookups self:: and parent:: static method calls.
        /// </summary>
        class LateStaticCallsLookup : GraphExplorer<bool>
        {
            List<MethodSymbol> _lazyStaticCalls;

            public static IList<MethodSymbol> GetSelfStaticCalls(BoundBlock block)
            {
                var visitor = new LateStaticCallsLookup();
                visitor.Accept(block);
                return (IList<MethodSymbol>)visitor._lazyStaticCalls ?? Array.Empty<MethodSymbol>();
            }
        }

        #endregion

        /// <summary>
        /// Walks static methods that don't use late static binding and checks if it should forward the late static type;
        /// hence it must know the late static as well.
        /// </summary>
        void ForwardLateStaticBindings()
        {
            var calls = new ConcurrentBag<KeyValuePair<SourceMethodSymbolBase, MethodSymbol>>();

            // collect self:: or parent:: static calls
            this.WalkSourceMethods(method =>
            {
                if (method is SourceMethodSymbol caller && caller.IsStatic &&
                    (caller.Flags & MethodFlags.UsesLateStatic) == 0)
                {
                    var cfg = caller.ControlFlowGraph;
                    if (cfg == null)
                    {
                        return;
                    }

                    // has self:: or parent:: call to a method?
                    var selfcalls = LateStaticCallsLookup.GetSelfStaticCalls(cfg.Start);
                    if (selfcalls.Count == 0)
                    {
                        return;
                    }

                    foreach (var callee in selfcalls)
                    {
                        calls.Add(new KeyValuePair<SourceMethodSymbolBase, MethodSymbol>(caller, callee));
                    }
                }
            }, allowParallel: ConcurrentBuild);

            // process edges between caller and calles until we forward all the late static calls
            int forwarded;
            do
            {
                forwarded = 0;
                Parallel.ForEach(calls, edge =>
                {
                    if ((edge.Key.Flags & MethodFlags.UsesLateStatic) == 0)
                    {
                        edge.Key.Flags |= MethodFlags.UsesLateStatic;
                        Interlocked.Increment(ref forwarded);
                    }
                });
            } while (forwarded != 0);
        }

        internal void DiagnoseMethods()
        {
            this.WalkSourceMethods(DiagnoseMethod, allowParallel: true);
        }

        private void DiagnoseMethod(SourceMethodSymbolBase method)
        {
            Contract.ThrowIfNull(method);

            DiagnosticWalker<VoidStruct>.Analyse(_diagnostics, method);
        }

        private void DiagnoseTypes()
        {
            this.WalkTypes(DiagnoseType, allowParallel: true);
        }
       
        private void DiagnoseType(SourceModuleTypeSymbol type)
        {
            type.GetDiagnostics(_diagnostics);
        }

        bool TransformMethods(bool allowParallel)
        {
            bool anyTransforms = false;
            var delayedTrn = new DelayedTransformations();

            this.WalkSourceMethods(m =>
                {
                    // Cannot be simplified due to multithreading ('=' is atomic unlike '|=')
                    if (TransformationRewriter.TryTransform(delayedTrn, m))
                        anyTransforms = true;
                },
                allowParallel: allowParallel);

            // Apply transformation that cannot run in the parallel way
            anyTransforms |= delayedTrn.Apply();

            return anyTransforms;
        }

        internal void EmitMethodBodies()
        {
            Debug.Assert(_moduleBuilder != null);

            // source methods
            this.WalkSourceMethods(this.EmitMethodBody, allowParallel: true);
        }

        internal void EmitSynthesized()
        {
            WalkSourceMethods(f => f.SynthesizeStubs(_moduleBuilder, _diagnostics));
            WalkSynthesizedMethods(m =>
                _moduleBuilder.SetMethodBody(m, m.CreateMethodBody(_moduleBuilder, _diagnostics)));
        }

        /// <summary>
        /// Generates analyzed method.
        /// </summary>
        void EmitMethodBody(SourceMethodSymbolBase method)
        {
            Contract.ThrowIfNull(method);

            if (method.ControlFlowGraph != null) // non-abstract method
            {
                Debug.Assert(method.ControlFlowGraph.Start.FlowState != null);

                var body = MethodGenerator.GenerateMethodBody(_moduleBuilder, method, 0, null, _diagnostics,
                    _emittingPdb);
                _moduleBuilder.SetMethodBody(method, body);
            }
        }

        void CompileEntryPoint()
        {
            if (_compilation.Options.OutputKind.IsApplication() && _moduleBuilder != null)
            {
                var entryPoint = _compilation.GetEntryPoint(_cancellationToken);
                if (entryPoint != null && !(entryPoint is ErrorMethodSymbol))
                {
                    _moduleBuilder.CreateEntryPoint((MethodSymbol)entryPoint, _diagnostics);

                    Debug.Assert(_moduleBuilder.EntryPointType.EntryPointSymbol != null);
                    _moduleBuilder.SetPEEntryPoint(_moduleBuilder.EntryPointType.EntryPointSymbol, _diagnostics);
                }
            }
        }

        bool MakeLoweringTransformMethods(bool allowParallel)
        {
            bool anyTransforms = false;
            this.WalkSourceMethods(m =>
                {
                    // Cannot be simplified due to multithreading ('=' is atomic unlike '|=')
                    if (LocalRewriter.TryTransform(m))
                        anyTransforms = true;
                },
                allowParallel: allowParallel);

            return anyTransforms;
        }


        private void InvalidateAndEnque(SourceMethodSymbolBase m)
        {
            m.ControlFlowGraph?.FlowContext?.InvalidateAnalysis();
            EnqueueMethod(m);
        }

        public void LoweringMethods()
        {
            if (MakeLoweringTransformMethods(ConcurrentBuild))
            {
                WalkSourceMethods(InvalidateAndEnque, allowParallel: true);
            }
        }

        bool RewriteMethods()
        {
            using (_compilation.StartMetric("transform"))
            {
                if (TransformMethods(ConcurrentBuild))
                {
                    WalkSourceMethods(InvalidateAndEnque, allowParallel: true);
                }
                else
                {
                    // No changes performed => no need to repeat the analysis
                    return false;
                }
            }
            
            return true;
        }

        public static IEnumerable<Diagnostic> BindAndAnalyze(AquilaCompilation compilation,
            CancellationToken cancellationToken)
        {
            var diagnostics = new DiagnosticBag();
            var compiler = new SourceCompiler(compilation, null, true, diagnostics, cancellationToken);

            using (compilation.StartMetric("metadata analysis"))
            {
                compiler.AnalyzeMetadata();
            }

            if (diagnostics.HasAnyErrors())
                return diagnostics.AsEnumerable();

            using (compilation.StartMetric("bind"))
            {
                // 1. Bind Syntax & Symbols to Operations (CFG)
                //   a. construct CFG, bind AST to Operation
                //   b. declare table of local variables
                compiler.WalkSourceMethods(compiler.EnqueueMethod, allowParallel: true);
            }

            if (diagnostics.HasAnyErrors())
                return diagnostics.AsEnumerable();

            // Repeat analysis and transformation until either the limit is met or there are no more changes
            int transformation = 0;
            do
            {
                using (compilation.StartMetric("analysis"))
                {
                    // Analyze Operations
                    compiler.AnalyzeMethods();
                }
                
                using (compilation.StartMetric("lowering"))
                {
                    // Lowering methods
                    compiler.LoweringMethods();
                }

                using (compilation.StartMetric(nameof(ForwardLateStaticBindings)))
                {
                    // Forward the late static type if needed
                    compiler.ForwardLateStaticBindings();
                }

                // Transform Semantic Trees for Runtime Optimization
            } while (
                transformation++ < compiler.MaxTransformCount // limit number of lowering cycles
                && !cancellationToken.IsCancellationRequested // user canceled ?
                && compiler.RewriteMethods()); // try lower the semantics

            // Track the number of actually performed transformations
            compilation.TrackMetric("transformations", transformation - 1);

            using (compilation.StartMetric("diagnostic"))
            {
                // Collect diagnostics
                compiler.DiagnoseMethods();
                compiler.DiagnoseTypes();
            }

            //
            return diagnostics.AsEnumerable();
        }

        private void AnalyzeMetadata()
        {
            var metadata = _compilation.MetadataProvider.GetSemanticMetadata();
            foreach (var md in metadata)
            {
                if (!md.IsValid)
                {
                    _diagnostics.Add(MessageProvider.Instance
                        .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, Location.None, md.Name));
                }
            }
        }

        public static void CompileSources(
            AquilaCompilation compilation,
            PEModuleBuilder moduleBuilder,
            bool emittingPdb,
            bool hasDeclarationErrors,
            DiagnosticBag diagnostics,
            CancellationToken cancellationToken)
        {
            Debug.Assert(moduleBuilder != null);

            compilation.TrackMetric("sourceFilesCount", compilation.SourceSymbolCollection.FilesCount);

            using (compilation.StartMetric("diagnostics"))
            {
                // ensure flow analysis and collect diagnostics
                var declarationDiagnostics = compilation.GetDeclarationDiagnostics(cancellationToken);
                diagnostics.AddRange(declarationDiagnostics);

                // cancel the operation if there are errors
                if (hasDeclarationErrors || declarationDiagnostics.HasAnyErrors() || cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }

            //
            var compiler = new SourceCompiler(compilation, moduleBuilder, emittingPdb, diagnostics, cancellationToken);

            using (compilation.StartMetric("emit"))
            {
                // Emit method bodies
                //   a. declared methods
                //   b. synthesized symbols
                compiler.EmitMethodBodies();
                compiler.EmitSynthesized();

                // Entry Point (.exe)
                compiler.CompileEntryPoint();
            }
        }
    }
}