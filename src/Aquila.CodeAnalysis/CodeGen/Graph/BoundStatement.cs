﻿using Aquila.CodeAnalysis.Symbols;
using System;
using Aquila.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Semantics
{
    // partial class BoundStatement : IGenerator
    // {
    //     internal virtual void Emit(CodeGenerator cg)
    //     {
    //         throw new NotImplementedException($"{this.GetType().Name}");
    //     }
    //
    //     void IGenerator.Generate(CodeGenerator cg) => Emit(cg);
    // }

    // partial class BoundEmptyStmt
    // {
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //         if (cg.EmitPdbSequencePoints && !_span.IsEmpty)
    //         {
    //             cg.EmitSequencePoint(_span);
    //         }
    //     }
    // }
    //
    // partial class BoundExpressionStmt
    // {
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //         if (Expression.IsConstant())
    //         {
    //             return;
    //         }
    //
    //         cg.EmitSequencePoint(this.AquilaSyntax);
    //         cg.EmitPop(Expression.Emit(cg));
    //     }
    // }
    //
    // partial class BoundReturnStmt
    // {
    //     internal override void Emit(CodeGenerator cg)
    //     {
    //         cg.Builder.AssertStackEmpty();
    //         cg.EmitSequencePoint(this.AquilaSyntax);
    //
    //         var rtype = cg.Method.ReturnType;
    //
    //         //
    //         if (this.Returned == null)
    //         {
    //             if (!rtype.IsVoid())
    //             {
    //                 // <default>
    //                 cg.EmitLoadDefault(rtype);
    //             }
    //         }
    //         else
    //         {
    //             cg.EmitConvert(this.Returned, rtype);
    //
    //             // TODO: check for null, if return type is not nullable
    //             if (cg.Method.SyntaxReturnType != null && !cg.Method.ReturnsNull)
    //             {
    //                 //// Template: Debug.Assert( <STACK> != null )
    //                 //cg.Builder.EmitOpCode(ILOpCode.Dup);
    //                 //cg.EmitNotNull(rtype, this.Returned.TypeRefMask);
    //                 //cg.EmitDebugAssert();
    //             }
    //         }
    //
    //         // .ret
    //         cg.EmitRet(rtype);
    //     }
    // }
}