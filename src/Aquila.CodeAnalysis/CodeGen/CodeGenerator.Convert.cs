using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.Synthesized;

namespace Aquila.CodeAnalysis.CodeGen
{
    partial class CodeGenerator
    {
        /// <summary>
        /// Emits expression and converts it to required type.
        /// </summary>
        public void EmitConvert(BoundExpression expr, TypeSymbol to,
            ConversionKind conversion = ConversionKind.Implicit, bool notNull = false)
        {
            Debug.Assert(expr != null);
            Debug.Assert(to != null);

            // pop effectively
            if (to.IsVoid())
            {
                expr.Access = BoundAccess.None;

                if (!expr.IsConstant())
                {
                    // POP LOAD <expr>
                    EmitPop(Emit(expr));
                }

                return;
            }

            EmitConvert(expr.Emit(this), to, conversion);
        }


        /// <summary>
        /// Emits conversion from one CLR type to another
        /// </summary>
        /// <param name="from">Type of value on top of evaluation stack.</param>
        /// <param name="to">Target CLR type.</param>
        /// <param name="conversion">Conversion semantic.</param>
        public void EmitConvert(TypeSymbol from, TypeSymbol to, ConversionKind conversion = ConversionKind.Implicit)
        {
            Contract.ThrowIfNull(from);
            Contract.ThrowIfNull(to);

            Debug.Assert(!from.IsUnreachable);
            Debug.Assert(!to.IsUnreachable);
            Debug.Assert(!to.IsErrorType(), "Conversion to an error type.");

            // conversion is not needed:
            if (from.SpecialType == to.SpecialType &&
                (from == to || (to.SpecialType != SpecialType.System_Object && from.IsOfType(to))))
            {
                return;
            }

            if (from.SpecialType == SpecialType.System_Void)
            {
                // void -> T
                EmitLoadDefault(to);
                return;
            }

            if (to is SynthesizedUnionTypeSymbol s)
            {
                var a = s.GetMembers("op_Implicit").OfType<MethodSymbol>().Where(
                    x => x.ReturnType == to
                         && x.ParameterCount == 1
                         && x.ParametersType()[0] == from);

                if (a.Any())
                {
                    EmitCall(ILOpCode.Call, a.First());
                    return;
                }
            }

            if (to.SpecialType == SpecialType.System_Object)
            {
                EmitOpCode(ILOpCode.Box);
                EmitSymbolToken(from, null);
                return;
            }

            throw new NotImplementedException();
        }

        public void EmitConvert(ITypeSymbol from, ITypeSymbol to, ConversionKind conversionKind = ConversionKind.Implicit) =>
            EmitConvert((TypeSymbol)from, (TypeSymbol)to, conversionKind);


        /// <summary>
        /// In case expression is of type <c>Int32</c> or <c>bool</c>
        /// converts it to <c>double</c> and leaves the result on evaluation stack. Otherwise
        /// just emits expression and leaves it on evaluation stack.
        /// </summary>
        internal TypeSymbol EmitExprConvertNumberToDouble(BoundExpression expr)
        {
            // emit number literal directly as double
            var constant = expr.ConstantValue;
            if (constant.HasValue)
            {
                if (constant.Value is long)
                {
                    _il.EmitDoubleConstant((long)constant.Value);
                    return this.CoreTypes.Double;
                }

                if (constant.Value is int)
                {
                    _il.EmitDoubleConstant((int)constant.Value);
                    return this.CoreTypes.Double;
                }

                if (constant.Value is bool)
                {
                    _il.EmitDoubleConstant((bool)constant.Value ? 1.0 : 0.0);
                    return this.CoreTypes.Double;
                }
            }
            
            var place = PlaceOrNull(expr);
            var type = (TypeSymbol)expr.Type;

            Debug.Assert(type != null);

            if (type.SpecialType == SpecialType.System_Int32 ||
                type.SpecialType == SpecialType.System_Int64 ||
                type.SpecialType == SpecialType.System_Boolean)
            {
                _il.EmitOpCode(ILOpCode.Conv_r8); // int|bool -> long
                type = this.CoreTypes.Double;
            }

            //
            return type;
        }
    }
}