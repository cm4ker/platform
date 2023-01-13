using Aquila.CodeAnalysis.Symbols;
using System;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis.Emit.NoPia;

namespace Aquila.CodeAnalysis.Semantics
{
    partial class BoundStatement : IGenerator
    {
        internal virtual void Emit(CodeGenerator cg)
        {
            throw new NotImplementedException($"{this.GetType().Name}");
        }

        void IGenerator.Generate(CodeGenerator cg) => Emit(cg);
    }

    partial class BoundEmptyStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            if (cg.EmitPdbSequencePoints && !_span.IsEmpty)
            {
                cg.EmitSequencePoint(_span);
            }
        }
    }

    partial class BoundExpressionStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            if (Expression.IsConstant())
            {
                return;
            }

            cg.EmitSequencePoint(this.AquilaSyntax);
            cg.EmitPop(Expression.Emit(cg));
        }
    }

    partial class BoundReturnStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            cg.Builder.AssertStackEmpty();
            cg.EmitSequencePoint(this.AquilaSyntax);

            var rtype = cg.Method.ReturnType;

            //
            if (this.Returned == null)
            {
                if (!rtype.IsVoid())
                {
                    // <default>
                    cg.EmitLoadDefault(rtype);
                }
            }
            else
            {
                cg.EmitConvert(this.Returned, rtype);
            }

            // .ret
            cg.EmitRet(rtype);
        }
    }

    partial class BoundHtmlOpenElementStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            var s = cg.Method.GetOrThrowViewMethod();
            
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitLoadConstant(this.InstructionIndex); 
            cg.EmitLoadConstant(this.ElementName);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.OpenElement);
        }
    }
    
    partial class BoundHtmlCloseElementStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            var s = cg.Method.GetOrThrowViewMethod();
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.CloseElement);
        }
    }

    partial class BoundHtmlAddAttributeStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            var s = cg.Method.GetOrThrowViewMethod();
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitLoadConstant(this.InstructionIndex);
            cg.EmitLoadConstant(this.AttributeName);
            cg.Emit(Expression);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.AddAttribute);
        }
    }
    
    partial class BoundHtmlMarkupStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            var s = cg.Method.GetOrThrowViewMethod();
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitLoadConstant(this.InstructionIndex);
            cg.EmitLoadConstant(this.Markup);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.AddMarkupContent);
        }
    }
}

