using Aquila.CodeAnalysis.Semantics;
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

        }

        private static bool TryGetVariableHandle(object boundvar, FlowState state,
            out VariableHandle varHandle)
        {
            varHandle = default(VariableHandle);
            return false;
        }
    }
}