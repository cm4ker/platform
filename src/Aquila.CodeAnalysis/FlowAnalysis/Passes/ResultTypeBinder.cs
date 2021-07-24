using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Microsoft.CodeAnalysis.Operations;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    /// <summary>
    /// Walks all expressions and resolves their access, operator method, and result CLR type.
    /// </summary>
    internal class ResultTypeBinder : GraphExplorer<TypeSymbol>
    {
        public AquilaCompilation DeclaringCompilation { get; }

        #region Initialization

        public ResultTypeBinder(AquilaCompilation compilation)
        {
            DeclaringCompilation = compilation ?? throw ExceptionUtilities.ArgumentNull(nameof(compilation));
        }

        public void Bind(SourceMethodSymbol method) => method.ControlFlowGraph?.Accept(this);

        public void Bind(SourceParameterSymbol parameter) => parameter.Initializer?.Accept(this);

        public void Bind(SourceFieldSymbol field) => field.Initializer?.Accept(this);

        #endregion

        /// <summary>
        /// Resolves access operator and updates <see cref="BoundExpression.BoundConversion"/>.
        /// </summary>
        /// <param name="expression">The expression which access has to be resolved.</param>
        /// <param name="type">The value type.</param>
        /// <param name="hasref">Whether we have the value by ref (addr).</param>
        /// <returns>Resulting expression type.</returns>
        TypeSymbol BindAccess(BoundExpression expression, TypeSymbol type, bool hasref)
        {
            var access = expression.Access;

            string opName = null;

            if (access.IsReadRef)
            {
            }
            else if (access.EnsureObject)
            {
                if (!type.IsReferenceType ||
                    Conversions.IsSpecialReferenceType(
                        type)) // keep Object as it is // TODO: just if it's safe (not NULL)
                {
                    opName = "EnsureObject";
                }
            }
            else if (access.EnsureArray)
            {
            }
            else
            {
                if (access.TargetType != null)
                {
                    expression.BoundConversion =
                        DeclaringCompilation.Conversions.ClassifyConversion(type, access.TargetType,
                            ConversionKind.Implicit);
                    // TODO: check in diagnostics the conversion exists
                    type = access.TargetType;
                }
            }

            // resolve the operator
            if (opName != null)
            {
                var op = DeclaringCompilation.Conversions.ResolveOperator(type, hasref, new[] { opName }, null);
                if (op != null)
                {
                    expression.BoundConversion = new CommonConversion(true, false, false, false, true, false, op);
                    type = op.ReturnType;
                }
                else
                {
                    throw new NotImplementedException($"Accessing '{type}' as {access}.");
                }
            }

            //
            return expression.ResultType = type;
        }
    }
}