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

            throw new NotImplementedException();
        }
    }
}