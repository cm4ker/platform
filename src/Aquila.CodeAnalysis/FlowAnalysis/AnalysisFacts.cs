using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.PE;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    static class AnalysisFacts
    {
        /// <summary>
        /// Ensures that the variable is of the given type(s) in the positive branch or not of this type in the negative
        /// branch. If the current branch is unfeasible, assigns an appropriate boolean to the
        /// <see cref="BoundExpression.ConstantValue"/> of <paramref name="checkExpr"/>.
        /// </summary>
        /// <param name="varRef">The reference to the variable whose types to check.</param>
        /// <param name="targetTypeCallback">The callback that receives the current type mask of the variable and returns
        /// the target one.</param>
        /// <param name="branch">The branch to check - <see cref="ConditionBranch.ToTrue"/> is understood as the positive
        /// branch if <paramref name="isPositiveCheck"/> is true.</param>
        /// <param name="flowState">The flow state of the branch.</param>
        /// <param name="skipPositiveIfAnyType">Whether to skip a mask with <see cref="TypeRefMask.IsAnyType"/> in the
        /// positive branch (in the negative branch, it is always skipped).</param>
        /// <param name="checkExpr">The expression to have its <see cref="BoundExpression.ConstantValue"/> potentially
        /// updated.</param>
        /// <param name="isPositiveCheck">Whether the expression returns true when the type check succeeds. For example,
        /// in the case of != it would be false.</param>
        public static void HandleTypeCheckingExpression(
            BoundVariableRef varRef,
            ConditionBranch branch,
            FlowState flowState,
            bool skipPositiveIfAnyType = false,
            BoundExpression checkExpr = null,
            bool isPositiveCheck = true)
        {
            if (!TryGetVariableHandle(varRef, flowState, out VariableHandle handle))
            {
                return;
            }

            var currentType = flowState.GetLocalType(handle);

            // Model negative type checks (such as $x != null) by inverting branches for the core checking function
            var branchHlp = isPositiveCheck ? branch : branch.NegativeBranch();

            // bool isFeasible = HandleTypeChecking(currentType, branchHlp, flowState, handle,
            //     skipPositiveIfAnyType);

            // If the constant value was not meant to be updated, skip its computation
            if (checkExpr == null)
            {
                return;
            }

            // if (!currentType.IsRef)
            // {
            //     // If the true branch proves to be unfeasible, the function always returns false and vice versa
            //     var resultConstVal = isFeasible
            //         ? default(Optional<object>)
            //         : new Optional<object>(!branch.TargetValue().Value);
            //
            //     // Each branch can clean only the constant value it produced during its analysis (in order not to lose result
            //     // of the other branch): true branch can produce false value and vice versa
            //     if (!resultConstVal.EqualsOptional(checkExpr.ConstantValue)
            //         && (resultConstVal.HasValue
            //             || checkExpr.ConstantValue.Value is false && branch == ConditionBranch.ToTrue
            //             || checkExpr.ConstantValue.Value is true && branch == ConditionBranch.ToFalse))
            //     {
            //         checkExpr.ConstantValue = resultConstVal;
            //     }
            // }
            // else
            // {
            //     // We cannot reason about the result of the check if the variable can be modified by reference
            //     checkExpr.ConstantValue = default(Optional<object>);
            // }
        }

        private static bool TryGetVariableHandle(object boundvar, FlowState state,
            out VariableHandle varHandle)
        {
            // if (boundvar is LocalVariableReference local && local.BoundName.IsDirect) // direct variable name
            // {
            //     if (local.VariableKind == VariableKind.LocalVariable ||
            //         local.VariableKind == VariableKind.Parameter ||
            //         local.VariableKind == VariableKind.LocalTemporalVariable)
            //     {
            //         varHandle = state.GetLocalHandle(local.BoundName.NameValue);
            //         return true;
            //     }
            // }

            //
            varHandle = default(VariableHandle);
            return false;
        }

        private static bool HandleTypeChecking(
            ConditionBranch branch,
            FlowState flowState,
            VariableHandle handle,
            bool skipTrueIfAnyType)
        {
            // Information whether this path can ever be taken
            bool isFeasible = true;

            return isFeasible;
        }

        /// <summary>
        /// Returns whether the given type can be used as an array key.
        /// </summary>
        public static bool IsValidKeyType(IBoundTypeRef type)
        {
            if (type is BoundPrimitiveTypeRef pt)
            {
                switch (pt.TypeCode)
                {
                    case AquilaTypeCode.Boolean:
                    case AquilaTypeCode.Long:
                    case AquilaTypeCode.Double:
                    case AquilaTypeCode.String:
                    case AquilaTypeCode.WritableString:
                    case AquilaTypeCode.Null:
                    case AquilaTypeCode.Mixed:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// If present, transforms the given constant value to a string corresponding to the key under which the item is stored in an array.
        /// </summary>
        /// <param name="keyConst">Constant value of the key.</param>
        /// <param name="key">If <paramref name="keyConst"/> contains a value, the key as a (string, long) tuple.
        /// The second item should be taken into account only if the first one is null.</param>
        /// <returns>Whether the value was constant at all.</returns>
        public static bool TryGetCanonicKeyStringConstant(Optional<object> keyConst, out (string, long) key)
        {
            if (!keyConst.HasValue)
            {
                key = default;
                return false;
            }

            var obj = keyConst.Value;

            if (obj == null)
            {
                key = ("", default);
            }
            else if (keyConst.TryConvertToLong(out long l))
            {
                key = (null, l);
            }
            else if (obj is string s)
            {
                key = (s, default);
            }
            else
            {
                key = default;
                return false;
            }

            return true;
        }
    }
}