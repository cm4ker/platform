using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Collections.Immutable;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Source;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.CodeGen
{
    internal partial class CodeGenerator : IDisposable
    {
        #region LocalScope

        internal class LocalScope
        {
            #region Fields

            readonly CodeGenerator _codegen;
            readonly LocalScope _parent;
            readonly ScopeType _type;
            readonly int _from, _to;

            SortedSet<BoundBlock> _blocks;

            #endregion

            #region Contruction

            public LocalScope(CodeGenerator codegen, LocalScope parent, ScopeType type, int from, int to)
            {
                Contract.ThrowIfNull(codegen);
                Debug.Assert(from < to);
                _codegen = codegen;
                _parent = parent;
                _type = type;
                _from = from;
                _to = to;
            }

            #endregion

            /// <summary>
            /// Gets underlaying <see cref="ILBuilder"/>.
            /// </summary>
            public ILBuilder IL => _codegen.Builder;

            /// <summary>
            /// Gets parent scope. Can be <c>null</c> in case of a root scope.
            /// </summary>
            public LocalScope Parent => _parent;

            public bool IsIn(BoundBlock block)
            {
                return block.Ordinal >= _from && block.Ordinal < _to;
            }

            public void Enqueue(BoundBlock block)
            {
                if (IsIn(block))
                {
                    if (_blocks == null)
                    {
                        _blocks = new SortedSet<BoundBlock>(BoundBlock.EmitOrderComparer.Instance);
                    }

                    _blocks.Add(block);
                }
                else
                {
                    if (Parent == null)
                        throw new ArgumentOutOfRangeException();

                    Parent.Enqueue(block);
                }
            }

            public void ContinueWith(BoundBlock block)
            {
                if (_codegen.IsGenerated(block))
                {
                    // backward edge;
                    // or the block was already emitted, branch there:
                    IL.EmitBranch(ILOpCode.Br, block);
                    return;
                }

                if (block.IsDead || block.FlowState == null)
                {
                    // ignore dead/unreachable blocks
                    return;
                }

                if (block.Ordinal < _from)
                {
                    throw new InvalidOperationException("block miss");
                }

                if (IsIn(block))
                {
                    // TODO: avoid branching to a guarded scope // e.g. goto x; try { x: }

                    if (_blocks == null || _blocks.Count == 0 || _blocks.Comparer.Compare(block, _blocks.First()) < 0)
                    {
                        if (_blocks != null)
                        {
                            _blocks.Remove(block);
                        }

                        // continue with the block
                        _codegen.GenerateBlock(block);
                        return;
                    }
                }

                // forward edge:
                // note: if block will follow immediately, .br will be ignored
                IL.EmitBranch(ILOpCode.Br, block);
                this.Enqueue(block);
            }

            internal BoundBlock Dequeue()
            {
                BoundBlock block = null;

                if (_blocks != null && _blocks.Count != 0)
                {
                    block = _blocks.First();
                    _blocks.Remove(block);
                }

                //
                return block;
            }

            internal void BlockGenerated(BoundBlock block)
            {
                // remove block from parent todo list
                var scope = this.Parent;
                while (scope != null)
                {
                    if (scope._blocks != null && scope._blocks.Remove(block))
                    {
                        return;
                    }

                    scope = scope.Parent;
                }
            }
        }

        #endregion

        #region Fields

        readonly ILBuilder _il;
        readonly SourceMethodSymbol _method;
        readonly PEModuleBuilder _moduleBuilder;
        readonly AquilaOptimizationLevel _optimizations;
        readonly bool _emitPdbSequencePoints;
        readonly DiagnosticBag _diagnostics;
        readonly DynamicOperationFactory _factory;

        /// <summary>
        /// Place for loading a reference to <c>Aquila.Core.AqContext</c>.
        /// </summary>
        readonly IPlace _contextPlace;

        /// <summary>
        /// Are locals initilized externally.
        /// </summary>
        readonly bool _localsInitialized;

        /// <summary>
        /// In case code generator emits body of a generator SM method,
        /// gets reference to synthesized method symbol with additional information.
        /// </summary>
        internal SourceGeneratorSymbol GeneratorStateMachineMethod
        {
            get => _smmethod;
            set => _smmethod = value;
        }

        SourceGeneratorSymbol _smmethod;

        /// <summary>
        /// Local variable containing the current state of state machine.
        /// </summary>
        internal LocalDefinition GeneratorStateLocal { get; set; }

        /// <summary>
        /// "finally" block to be branched in when returning from the method.
        /// This finally block is not handled by CLR as it is emitted outside the TryCatchFinally scope.
        /// </summary>
        internal BoundBlock ExtraFinallyBlock { get; set; }

        internal enum ExtraFinallyState : int
        {
            /// <summary>continue to NextBlock</summary>
            None = 0,

            /// <summary>continue to next ExtraFinallyBlock, eventually EmitRet</summary>
            Return = 1,

            /// <summary>rethrow exception (<see cref="ExceptionToRethrowVariable"/>)</summary>
            Exception = 2,
        }

        /// <summary>
        /// Temporary variable holding state of "finally" block handling. Value of <see cref="ExtraFinallyState"/>.
        /// Variable created once only if <see cref="ExtraFinallyBlock"/> is set.
        /// Type: <c>System.Int32</c>
        /// </summary>
        internal TemporaryLocalDefinition ExtraFinallyStateVariable { get; set; }

        /// <summary>
        /// Local variable with array of all method's arguments.
        /// Initialized once when <see cref="SourceMethodSymbol.Flags"/> has <see cref="MethodFlags.UsesArgs"/>.
        /// </summary>
        internal LocalDefinition FunctionArgsArray { get; set; }

        /// <summary>
        /// BoundBlock.Tag value indicating the block was emitted.
        /// </summary>
        readonly int _emmittedTag;

        LocalScope _scope = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets underlaying <see cref="ILBuilder"/>.
        /// </summary>
        public ILBuilder Builder => _il;

        /// <summary>
        /// Module builder.
        /// </summary>
        public PEModuleBuilder Module => _moduleBuilder;

        /// <summary>
        /// Gets the method we are emitting.
        /// </summary>
        public SourceMethodSymbol Method => _method;

        /// <summary>
        /// For debug purposes.
        /// Current method being generated.
        /// </summary>
        internal MethodSymbol DebugMethod { get; set; }

        /// <summary>
        /// Type context of currently emitted expressions. Can be <c>null</c>.
        /// </summary>
        internal TypeRefContext TypeRefContext
        {
            get => _typeRefContext ?? this.Method?.TypeRefContext;
            set => _typeRefContext = value;
        }

        TypeRefContext _typeRefContext;

        public DiagnosticBag Diagnostics => _diagnostics;

        /// <summary>
        /// Whether to emit debug assertions.
        /// </summary>
        public bool IsDebug => _optimizations.IsDebug();

        /// <summary>
        /// Whether to emit sequence points (PDB).
        /// </summary>
        public bool EmitPdbSequencePoints => _emitPdbSequencePoints;

        /// <summary>
        /// Gets a reference to compilation object.
        /// </summary>
        public AquilaCompilation DeclaringCompilation => _moduleBuilder.Compilation;

        /// <summary>Gets <see cref="PrimitiveBoundTypeRefs"/> instance.</summary>
        PrimitiveBoundTypeRefs PrimitiveBoundTypeRefs => DeclaringCompilation.TypeRefs;

        /// <summary>
        /// Gets conversions helper class.
        /// </summary>
        public Conversions Conversions => DeclaringCompilation.Conversions;

        /// <summary>
        /// Well known types.
        /// </summary>
        public CoreTypes CoreTypes => DeclaringCompilation.CoreTypes;

        // /// <summary>
        // /// Well known methods.
        // /// </summary>
        // public CoreMethods CoreMethods => DeclaringCompilation.CoreMethods;

        /// <summary>
        /// Factory for dynamic and anonymous types.
        /// </summary>
        public DynamicOperationFactory Factory => _factory;

        /// <summary>
        /// Whether the generator corresponds to a global scope.
        /// </summary>
        public bool IsGlobalScope => false;


        /// <summary>
        /// Gets or sets value determining <see cref="BoundArrayEx"/> is being emitted.
        /// When set, array expression caching is disabled.
        /// </summary>
        internal bool IsInCachedArrayExpression { get; set; }

        /// <summary>
        /// Type of the caller context (the class declaring current method) or null.
        /// </summary>
        public TypeSymbol CallerType
        {
            get => GetSelfType(_callerType ?? (_method is SourceMethodSymbol ? _method.ContainingType : null));
            set => _callerType = value;
        }

        TypeSymbol _callerType;

        static TypeSymbol GetSelfType(TypeSymbol scope) =>
            throw new NotImplementedException(); //scope is SourceTraitTypeSymbol t) ? t.TSelfParameter : scope;

        // public SourceFileSymbol ContainingFile
        // {
        //     get => _containingFile;
        //     internal set => _containingFile = value;
        // }
        //
        // SourceFileSymbol _containingFile;

        internal ExitBlock ExitBlock => ((ExitBlock)this.Method.ControlFlowGraph.Exit);

        #endregion

        #region Construction

        public CodeGenerator(ILBuilder il, PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics,
            AquilaOptimizationLevel optimizations, bool emittingPdb,
            NamedTypeSymbol container, IPlace contextPlace, MethodSymbol method = null,
            bool localsInitialized = false)
        {
            Contract.ThrowIfNull(il);
            Contract.ThrowIfNull(moduleBuilder);


            _il = il;
            _moduleBuilder = moduleBuilder;
            _optimizations = optimizations;
            _diagnostics = diagnostics;

            _localsInitialized = localsInitialized;

            _emmittedTag = 0;

            _contextPlace = contextPlace;

            _emitPdbSequencePoints = emittingPdb;

            _method = method as SourceMethodSymbol;

            var syntax = _method?.Syntax;

            if (EmitPdbSequencePoints && syntax != null)
            {
                il.SetInitialDebugDocument(syntax.SyntaxTree);
            }
        }

        /// <summary>
        /// Copy ctor with different method content (and TypeRefContext).
        /// Used for emitting in a context of a different method (parameter initializer).
        /// </summary>
        public CodeGenerator(CodeGenerator cg, SourceMethodSymbol method)
            : this(cg._il, cg._moduleBuilder, cg._diagnostics, cg._optimizations, cg._emitPdbSequencePoints,
                method.ContainingType, cg._contextPlace, method)
        {
            _emmittedTag = cg._emmittedTag;
            GeneratorStateLocal = cg.GeneratorStateLocal;
            ExtraFinallyBlock = cg.ExtraFinallyBlock;
        }

        public CodeGenerator(SourceMethodSymbol method, ILBuilder il, PEModuleBuilder moduleBuilder,
            DiagnosticBag diagnostics, AquilaOptimizationLevel optimizations, bool emittingPdb)
            : this(il, moduleBuilder, diagnostics, optimizations, emittingPdb, method.ContainingType, method.GetContextPlace(moduleBuilder), method)
        {
            Contract.ThrowIfNull(method);

            _emmittedTag = (method.ControlFlowGraph != null) ? method.ControlFlowGraph.NewColor() : -1;

            bool localsAlreadyInited;


            // Emit sequence points unless
            // - the PDBs are not being generated
            // - debug information for the method is not generated since the method does not contain
            //   user code that can be stepped through, or changed during EnC.
            // 
            // This setting only affects generating PDB sequence points, it shall not affect generated IL in any way.
            _emitPdbSequencePoints = emittingPdb && true; // method.GenerateDebugInfo;
        }

        #endregion

        #region CFG Emitting

        /// <summary>
        /// Emits methods body.
        /// </summary>
        internal void Generate()
        {
            Debug.Assert(_method != null && _method.ControlFlowGraph != null);
            _method.Generate(this);
        }

        internal void GenerateScope(BoundBlock block, int to)
        {
            GenerateScope(block, ScopeType.Variable, to);
        }

        internal void GenerateScope(BoundBlock block, ScopeType type, int to)
        {
            Contract.ThrowIfNull(block);

            // open scope
            _scope = new LocalScope(this, _scope, type, block.Ordinal, to);
            _scope.ContinueWith(block);

            while ((block = _scope.Dequeue()) != null)
            {
                GenerateBlock(block);
            }

            // close scope
            _scope = _scope.Parent;

            //
            _il.AssertStackEmpty();
        }

        /// <summary>
        /// Gets a reference to the current scope.
        /// </summary>
        internal LocalScope Scope => _scope;

        void GenerateBlock(BoundBlock block)
        {
            // mark the block as emitted
            Debug.Assert(block.Tag != _emmittedTag);
            block.Tag = _emmittedTag;

            // mark location as a label
            // to allow branching to the block
            _il.MarkLabel(block);

            //
            _scope.BlockGenerated(block);

            //
            Generate(block);
        }

        /// <summary>
        /// Invokes <see cref="IGenerator.Generate"/>.
        /// </summary>
        internal void Generate(IGenerator element)
        {
            element?.Generate(this);
        }

        /// <summary>
        /// Gets value indicating whether the given block was already emitted.
        /// </summary>
        internal bool IsGenerated(BoundBlock block)
        {
            Contract.ThrowIfNull(block);
            return block.Tag == _emmittedTag;
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents a semantic element that can be emitted.
    /// </summary>
    internal interface IGenerator
    {
        /// <summary>
        /// Emits IL into the underlaying <see cref="ILBuilder"/>.
        /// </summary>
        void Generate(CodeGenerator cg);
    }

    [DebuggerDisplay("Label {_name}")]
    internal sealed class NamedLabel
    {
        readonly string _name;

        public NamedLabel(string name)
        {
            _name = name;
        }
    }
}