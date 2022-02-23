﻿using Microsoft.CodeAnalysis;
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
            SourceMethodSymbol method,
            int methodOrdinal,
            //ImmutableArray<LambdaDebugInfo> lambdaDebugInfo,
            //ImmutableArray<ClosureDebugInfo> closureDebugInfo,
            //StateMachineTypeSymbol stateMachineTypeOpt,
            VariableSlotAllocator variableSlotAllocatorOpt,
            DiagnosticBag diagnostics,
            //ImportChain importChainOpt,
            bool emittingPdb)
        {
            return GenerateMethodBody(moduleBuilder, method, (builder) =>
            {
                var optimization = moduleBuilder.Compilation.Options.OptimizationLevel;
                var codeGen = new CodeGenerator(method, builder, moduleBuilder, diagnostics, optimization,
                    emittingPdb);

                // We need to save additional debugging information for MoveNext of an async state machine.
                //var stateMachineMethod = method as SynthesizedStateMachineMethod;
                //bool isStateMachineMoveNextMethod = stateMachineMethod != null && method.Name == WellKnownMemberNames.MoveNextMethodName;

                //if (isStateMachineMoveNextMethod && stateMachineMethod.StateMachineType.KickoffMethod.IsAsync)
                //{
                //    int asyncCatchHandlerOffset;
                //    ImmutableArray<int> asyncYieldPoints;
                //    ImmutableArray<int> asyncResumePoints;
                //    codeGen.Generate(out asyncCatchHandlerOffset, out asyncYieldPoints, out asyncResumePoints);

                //    var kickoffMethod = stateMachineMethod.StateMachineType.KickoffMethod;

                //    // The exception handler IL offset is used by the debugger to treat exceptions caught by the marked catch block as "user unhandled".
                //    // This is important for async void because async void exceptions generally result in the process being terminated,
                //    // but without anything useful on the call stack. Async Task methods on the other hand return exceptions as the result of the Task.
                //    // So it is undesirable to consider these exceptions "user unhandled" since there may well be user code that is awaiting the task.
                //    // This is a heuristic since it's possible that there is no user code awaiting the task.
                //    asyncDebugInfo = new Cci.AsyncMethodBodyDebugInfo(kickoffMethod, kickoffMethod.ReturnsVoid ? asyncCatchHandlerOffset : -1, asyncYieldPoints, asyncResumePoints);
                //}
                //else
                {
                    codeGen.Generate();
                }
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

                //
                il.Realize();

                //
                var localVariables = il.LocalSlotManager.LocalsInOrder();

                if (localVariables.Length > 0xFFFE)
                {
                    //diagnosticsForThisMethod.Add(ErrorCode.ERR_TooManyLocals, method.Locations.First());
                }

                //if (diagnosticsForThisMethod.HasAnyErrors())
                //{
                //    // we are done here. Since there were errors we should not emit anything.
                //    return null;
                //}

                //// We will only save the IL builders when running tests.
                //if (moduleBuilder.SaveTestData)
                //{
                //    moduleBuilder.SetMethodTestData(method, builder.GetSnapshot());
                //}

                // Only compiler-generated MoveNext methods have iterator scopes.  See if this is one.
                var stateMachineHoistedLocalScopes = default(ImmutableArray<StateMachineHoistedLocalScope>);
                //if (isStateMachineMoveNextMethod)
                //{
                //    stateMachineHoistedLocalScopes = builder.GetHoistedLocalScopes();
                //}

                var stateMachineHoistedLocalSlots = default(ImmutableArray<EncHoistedLocalInfo>);
                var stateMachineAwaiterSlots = default(ImmutableArray<Cci.ITypeReference>);
                //if (optimizations == OptimizationLevel.Debug && stateMachineTypeOpt != null)
                //{
                //    Debug.Assert(method.IsAsync || method.IsIterator);
                //    GetStateMachineSlotDebugInfo(moduleBuilder, moduleBuilder.GetSynthesizedFields(stateMachineTypeOpt), variableSlotAllocatorOpt, diagnosticsForThisMethod, out stateMachineHoistedLocalSlots, out stateMachineAwaiterSlots);
                //    Debug.Assert(!diagnostics.HasAnyErrors());
                //}

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
                    null, // importScopeOpt,
                    ImmutableArray<LambdaDebugInfo>.Empty, // lambdaDebugInfo,
                    ImmutableArray<ClosureDebugInfo>.Empty, // closureDebugInfo,
                    null, //stateMachineTypeOpt?.Name,
                    stateMachineHoistedLocalScopes,
                    stateMachineHoistedLocalSlots,
                    stateMachineAwaiterSlots,
                    stateMachineMoveNextDebugInfo,
                    null); // dynamicAnalysisDataOpt
            }
            finally
            {
                // Basic blocks contain poolable builders for IL and sequence points. Free those back
                // to their pools.
                il.FreeBasicBlocks();

                //// Remember diagnostics.
                //diagnostics.AddRange(diagnosticsForThisMethod);
                //diagnosticsForThisMethod.Free();
            }
        }

        internal static MethodBody CreateSynthesizedBody(PEModuleBuilder moduleBuilder, IMethodSymbol method,
            ILBuilder il)
        {
            var compilation = moduleBuilder.Compilation;
            var localSlotManager = il.LocalSlotManager;
            var optimizations = compilation.Options.OptimizationLevel;

            try
            {
                il.Realize();

                //
                var localVariables = il.LocalSlotManager.LocalsInOrder();

                if (localVariables.Length > 0xFFFE)
                {
                    //diagnosticsForThisMethod.Add(ErrorCode.ERR_TooManyLocals, method.Locations.First());
                }

                //if (diagnosticsForThisMethod.HasAnyErrors())
                //{
                //    // we are done here. Since there were errors we should not emit anything.
                //    return null;
                //}

                //// We will only save the IL builders when running tests.
                //if (moduleBuilder.SaveTestData)
                //{
                //    moduleBuilder.SetMethodTestData(method, builder.GetSnapshot());
                //}

                // Only compiler-generated MoveNext methods have iterator scopes.  See if this is one.
                var stateMachineHoistedLocalScopes = default(ImmutableArray<StateMachineHoistedLocalScope>);
                //if (isStateMachineMoveNextMethod)
                //{
                //    stateMachineHoistedLocalScopes = builder.GetHoistedLocalScopes();
                //}

                var stateMachineHoistedLocalSlots = default(ImmutableArray<EncHoistedLocalInfo>);
                var stateMachineAwaiterSlots = default(ImmutableArray<Cci.ITypeReference>);
                //if (optimizations == OptimizationLevel.Debug && stateMachineTypeOpt != null)
                //{
                //    Debug.Assert(method.IsAsync || method.IsIterator);
                //    GetStateMachineSlotDebugInfo(moduleBuilder, moduleBuilder.GetSynthesizedFields(stateMachineTypeOpt), variableSlotAllocatorOpt, diagnosticsForThisMethod, out stateMachineHoistedLocalSlots, out stateMachineAwaiterSlots);
                //    Debug.Assert(!diagnostics.HasAnyErrors());
                //}

                return new MethodBody(
                    il.RealizedIL,
                    il.MaxStack,
                    (Cci.IMethodDefinition)method,
                    new DebugId(0, moduleBuilder.CurrentGenerationOrdinal),
                    localVariables,
                    il.RealizedSequencePoints,
                    null,
                    il.RealizedExceptionHandlers,
                    il.AreLocalsZeroed,
                    hasStackalloc: false, // No support for stackalloc
                    il.GetAllScopes(),
                    il.HasDynamicLocal,
                    null, // importScopeOpt,
                    ImmutableArray<LambdaDebugInfo>.Empty, // lambdaDebugInfo,
                    ImmutableArray<ClosureDebugInfo>.Empty, // closureDebugInfo,
                    null, //stateMachineTypeOpt?.Name,
                    stateMachineHoistedLocalScopes,
                    stateMachineHoistedLocalSlots,
                    stateMachineAwaiterSlots,
                    null, // stateMachineMoveNextDebugInfoOpt
                    null); // dynamicAnalysisDataOpt
            }
            finally
            {
                // Basic blocks contain poolable builders for IL and sequence points. Free those back
                // to their pools.
                il.FreeBasicBlocks();

                //// Remember diagnostics.
                //diagnostics.AddRange(diagnosticsForThisMethod);
                //diagnosticsForThisMethod.Free();
            }
        }
    }
}