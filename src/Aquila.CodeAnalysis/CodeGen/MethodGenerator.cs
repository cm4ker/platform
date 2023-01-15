using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.CodeGen
{
    internal sealed class MethodGenerator
    {
        static Cci.DebugSourceDocument CreateDebugSourceDocument(string normalizedPath, MethodSymbol method)
        {
            // TODO: method might be synthesized and we create an incomplete DebugSourceDocument

            var srcf = (method as SourceMethodSymbol)?.Syntax;
            if (srcf != null && srcf.SyntaxTree.TryGetText(out var srctext))
            {
                return new Cci.DebugSourceDocument(
                    normalizedPath,
                    LanguageConstants.CorSymLanguageTypeAquila,
                    srctext.GetChecksum(),
                    SourceHashAlgorithms.GetAlgorithmGuid(srctext.ChecksumAlgorithm));
            }
            else
            {
                return new Cci.DebugSourceDocument(normalizedPath, LanguageConstants.CorSymLanguageTypeAquila);
            }
        }

        internal static MethodBody GenerateMethodBody(
            PEModuleBuilder moduleBuilder,
            SourceMethodSymbolBase method,
            int methodOrdinal, VariableSlotAllocator variableSlotAllocatorOpt,
            DiagnosticBag diagnostics, bool emittingPdb)
        {
            return GenerateMethodBody(moduleBuilder, method, (builder) =>
            {
                var optimization = moduleBuilder.Compilation.Options.OptimizationLevel;
                var codeGen = new CodeGenerator(method, builder, moduleBuilder, diagnostics, optimization,
                    emittingPdb);

                codeGen.Generate();
            }, variableSlotAllocatorOpt, diagnostics, emittingPdb);
        }

        /// <summary>
        /// Generates method body that calls another method.
        /// Used for wrapping a method call into a method, e.g. an entry point.
        /// </summary>
        internal static MethodBody GenerateMethodBody(
            PEModuleBuilder moduleBuilder,
            MethodSymbol method,
            Action<ILBuilder> builder,
            VariableSlotAllocator variableSlotAllocatorOpt,
            DiagnosticBag diagnostics,
            bool emittingPdb)
        {
            var compilation = moduleBuilder.Compilation;
            var localSlotManager = new LocalSlotManager(variableSlotAllocatorOpt);
            var optimizations = compilation.Options.OptimizationLevel;

            DebugDocumentProvider debugDocumentProvider = null;

            if (emittingPdb)
            {
                debugDocumentProvider = (path, basePath) =>
                {
                    return moduleBuilder.DebugDocumentsBuilder.GetOrAddDebugDocument(
                        path,
                        basePath,
                        normalizedPath => CreateDebugSourceDocument(normalizedPath, method));
                };
            }

            // TODO: Check whether in some cases we cannot skip it
            bool areLocalsZeroed = true;

            ILBuilder il = new ILBuilder(moduleBuilder, localSlotManager, optimizations.AsOptimizationLevel(),
                areLocalsZeroed);
            try
            {
                StateMachineMoveNextBodyDebugInfo stateMachineMoveNextDebugInfo = null;

                builder(il);
                il.Realize();
                var localVariables = il.LocalSlotManager.LocalsInOrder();
                
                // Only compiler-generated MoveNext methods have iterator scopes.  See if this is one.
                var stateMachineHoistedLocalScopes = default(ImmutableArray<StateMachineHoistedLocalScope>);
                
                var stateMachineHoistedLocalSlots = default(ImmutableArray<EncHoistedLocalInfo>);
                var stateMachineAwaiterSlots = default(ImmutableArray<Cci.ITypeReference>);
                

                return new MethodBody(
                    il.RealizedIL,
                    il.MaxStack,
                    (Cci.IMethodDefinition)method.PartialDefinitionPart ?? method,
                    variableSlotAllocatorOpt?.MethodId ?? new DebugId(0, moduleBuilder.CurrentGenerationOrdinal),
                    localVariables,
                    il.RealizedSequencePoints,
                    debugDocumentProvider,
                    il.RealizedExceptionHandlers,
                    il.AreLocalsZeroed,
                    hasStackalloc: false,
                    il.GetAllScopes(),
                    il.HasDynamicLocal,
                    importScopeOpt: null, 
                    ImmutableArray<LambdaDebugInfo>.Empty,
                    ImmutableArray<ClosureDebugInfo>.Empty,
                    stateMachineTypeNameOpt: null,
                    stateMachineHoistedLocalScopes,
                    stateMachineHoistedLocalSlots,
                    stateMachineAwaiterSlots,
                    stateMachineMoveNextDebugInfo,
                    dynamicAnalysisDataOpt: null);
            }
            finally
            {
                // Basic blocks contain poolable builders for IL and sequence points. Free those back
                // to their pools.
                il.FreeBasicBlocks();
            }
        }

        internal static MethodBody CreateSynthesizedBody(PEModuleBuilder moduleBuilder, IMethodSymbol method,
            ILBuilder il)
        {
            try
            {
                il.Realize();

                //
                var localVariables = il.LocalSlotManager.LocalsInOrder();

                // Only compiler-generated MoveNext methods have iterator scopes.  See if this is one.
                var stateMachineHoistedLocalScopes = default(ImmutableArray<StateMachineHoistedLocalScope>);
                var stateMachineHoistedLocalSlots = default(ImmutableArray<EncHoistedLocalInfo>);
                var stateMachineAwaiterSlots = default(ImmutableArray<Cci.ITypeReference>);

                return new MethodBody(
                    il.RealizedIL,
                    il.MaxStack,
                    (Cci.IMethodDefinition)method,
                    new DebugId(0, moduleBuilder.CurrentGenerationOrdinal),
                    localVariables,
                    il.RealizedSequencePoints,
                    debugDocumentProvider: null,
                    il.RealizedExceptionHandlers,
                    il.AreLocalsZeroed,
                    hasStackalloc: false,
                    il.GetAllScopes(),
                    il.HasDynamicLocal,
                    importScopeOpt: null, 
                    ImmutableArray<LambdaDebugInfo>.Empty, // lambdaDebugInfo,
                    ImmutableArray<ClosureDebugInfo>.Empty, // closureDebugInfo,
                    stateMachineTypeNameOpt: null,
                    stateMachineHoistedLocalScopes,
                    stateMachineHoistedLocalSlots,
                    stateMachineAwaiterSlots,
                    stateMachineMoveNextDebugInfoOpt:null,
                    dynamicAnalysisDataOpt:null);
            }
            finally
            {
                // Basic blocks contain poolable builders for IL and sequence points. Free those back
                // to their pools.
                il.FreeBasicBlocks();
            }
        }
    }
}