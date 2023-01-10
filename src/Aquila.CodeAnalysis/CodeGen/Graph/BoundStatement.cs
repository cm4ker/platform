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

                // TODO: check for null, if return type is not nullable
                if (cg.Method.SyntaxReturnType != null && !cg.Method.ReturnsNull)
                {
                    //// Template: Debug.Assert( <STACK> != null )
                    //cg.Builder.EmitOpCode(ILOpCode.Dup);
                    //cg.EmitNotNull(rtype, this.Returned.TypeRefMask);
                    //cg.EmitDebugAssert();
                }
            }

            // .ret
            cg.EmitRet(rtype);
        }
    }

    partial class BoundHtmlOpenElementStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            if (cg.Method is not SourceViewTypeSymbol.MethodTreeBuilderSymbol s)
            {
                throw new InvalidOperationException(
                    $"Only {nameof(SourceViewTypeSymbol.MethodTreeBuilderSymbol)} can be a host for this instruction");
            }
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitLoadConstant(1); //TODO: Insert index of operation
            cg.EmitLoadConstant(this.ElementName);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.OpenElement);
        }
    }
    
    partial class BoundHtmlCloseElementStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            if (cg.Method is not SourceViewTypeSymbol.MethodTreeBuilderSymbol s)
            {
                throw new InvalidOperationException(
                    $"Only {nameof(SourceViewTypeSymbol.MethodTreeBuilderSymbol)} can be a host for this instruction");
            }
            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.CloseElement);
        }
    }

    partial class BoundHtmlAddAttributeStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            if (cg.Method is not SourceViewTypeSymbol.MethodTreeBuilderSymbol s)
            {
                throw new InvalidOperationException(
                    $"Only {nameof(SourceViewTypeSymbol.MethodTreeBuilderSymbol)} can be a host for this instruction");
            }

            s.GetBuilderPlace().EmitLoad(cg.Builder);
            cg.EmitLoadConstant(1);
            cg.Emit(Expression);
            cg.EmitCall(ILOpCode.Call, cg.CoreMethods.RenderTreeBuilder.AddAttribute);
        }
    }
    
    partial class BoundHtmlMarkupStmt
    {
        internal override void Emit(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }
    }
}

