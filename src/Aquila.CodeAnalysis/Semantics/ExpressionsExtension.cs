﻿using Aquila.Syntax;

namespace Aquila.CodeAnalysis.Semantics
{
    static class ExpressionsExtension
    {
        public static T WithAccess<T>(this T expr, BoundAccess access) where T : BoundExpression
        {
            expr.Access = access;
            return expr;
        }

        public static T WithAccess<T>(this T expr, BoundExpression other) where T : BoundExpression =>
            WithAccess(expr, other.Access);

        public static T WithSyntax<T>(this T expr, AquilaSyntaxNode syntax) where T : IAquilaOperation
        {
            expr.AquilaSyntax = syntax;
            return expr;
        }

        /// <summary>
        /// Copies semantic information (<see cref="BoundExpression.Access"/>, <see cref="BoundExpression.AquilaSyntax"/>) from another expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T WithContext<T>(this T expr, BoundExpression other) where T : BoundExpression
        {
            expr.Access = other.Access;
            expr.AquilaSyntax = other.AquilaSyntax;

            return expr;
        }

        /// <summary>
        /// Gets value indicating the object will be an empty string after converting to string.
        /// </summary>
        public static bool IsEmptyStringValue(object value)
        {
            if (value == null)
                return true;

            if (value is string str && str.Length == 0)
                return true;

            if (value is bool b && b == false)
                return true;

            return false;
        }

        /// <summary>
        /// Returns whether the expression can possibly have any side effects.
        /// </summary>
        public static bool CanHaveSideEffects(this BoundExpression expr) =>  
            // TODO: Make more precise and less defensive
            !(expr.ConstantValue.HasValue
              || expr is BoundVariableRef varExpr && varExpr.Name.IsDirect
              || expr is BoundLiteral);
    }
}