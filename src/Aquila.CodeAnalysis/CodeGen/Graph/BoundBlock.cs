using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    // partial class BoundBlock : IGenerator
    // {
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //         // emit contained statements
    //         if (_statements.Count != 0)
    //         {
    //             _statements.ForEach(cg.Generate);
    //         }
    //
    //         //
    //         cg.Generate(this.NextEdge);
    //     }
    //
    //     void IGenerator.Generate(CodeGenerator cg) => Emit(cg);
    //
    //     /// <summary>
    //     /// Helper comparer defining order in which are blocks emitted if there is more than one in the queue.
    //     /// Can be used for optimizing branches heuristically.
    //     /// </summary>
    //     internal sealed class EmitOrderComparer : IComparer<BoundBlock>
    //     {
    //         // TODO: blocks emit priority
    //
    //         public static readonly EmitOrderComparer Instance = new EmitOrderComparer();
    //
    //         private EmitOrderComparer()
    //         {
    //         }
    //
    //         public int Compare(BoundBlock x, BoundBlock y) => x.Ordinal - y.Ordinal;
    //     }
    // }

    // partial class StartBlock
    // {
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //         cg.Builder.DefineInitialHiddenSequencePoint();
    //
    //         var locals = cg.Method.LocalsTable;
    //
    //         // variables/parameters initialization
    //         foreach (var loc in locals.Variables)
    //         {
    //             loc.EmitInit(cg);
    //         }
    //
    //         base.Emit(cg);
    //     }
    // }

    // partial class ExitBlock
    // {
    //     /// <summary>
    //     /// Temporary local variable for return.
    //     /// </summary>
    //     private Microsoft.CodeAnalysis.CodeGen.LocalDefinition _rettmp;
    //
    //     /// <summary>
    //     /// Return label.
    //     /// </summary>
    //     private object _retlbl;
    //
    //     /// <summary>
    //     /// Rethrow label.
    //     /// Marks code that rethrows <see cref="CodeGenerator.ExceptionToRethrowVariable"/>.
    //     /// </summary>
    //     private object _throwlbl;
    //
    //     internal object GetReturnLabel()
    //     {
    //         return _retlbl ??= new NamedLabel("<return>");
    //     }
    //
    //     internal object GetRethrowLabel()
    //     {
    //         return _throwlbl ??= new NamedLabel("<rethrow>");
    //     }
    //
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //     }
    //
    //     /// <summary>
    //     /// Stores value from top of the evaluation stack to a temporary variable which will be returned from the exit block.
    //     /// </summary>
    //     internal void EmitTmpRet(CodeGenerator cg, TypeSymbol stack, bool yielding)
    //     {
    //         if (_rettmp == null)
    //         {
    //             var rtype = cg.Method.ReturnType;
    //             if (rtype.SpecialType != SpecialType.System_Void)
    //             {
    //                 _rettmp = cg.GetTemporaryLocal(rtype);
    //             }
    //         }
    //
    //         // <rettmp> = <stack>;
    //         if (_rettmp != null)
    //         {
    //             cg.EmitConvert(stack, (TypeSymbol)_rettmp.Type);
    //             cg.Builder.EmitLocalStore(_rettmp);
    //         }
    //         else
    //         {
    //             cg.EmitPop(stack);
    //         }
    //
    //         //
    //         if (cg.ExtraFinallyBlock != null && !yielding)
    //         {
    //             // state = 1;
    //             // goto _finally;
    //             cg.Builder.EmitIntConstant((int)CodeGenerator.ExtraFinallyState.Return); // 1: return
    //             cg.ExtraFinallyStateVariable.EmitStore();
    //             cg.Builder.EmitBranch(ILOpCode.Br, cg.ExtraFinallyBlock);
    //             return;
    //         }
    //
    //         //
    //         cg.Builder.EmitBranch(ILOpCode.Br, GetReturnLabel());
    //     }
    // }
}