using System;
using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public class TypedExprFactory
    {
        public static MultiTypedExpr CreateMultiTypedExpr(QExpression expr, QueryMachine qm, RealWalkerBase walker)
        {
            return expr switch
            {
                QField f => new MultiTypeQFieldExpression(f, walker, qm),
                QCase @case => new MultiTypeCaseExpression(@case, walker, qm),
                QParameter o => new MultiTypeQParameterExpression(o, walker, qm),
                QTypedParameter p => new MultiTypeQTypedParameterExpression(p, walker, qm),
                _ => throw new Exception($"Typed expression {expr} not supported")
            };
        }

        public static SingleTypeExpr CreateSingleTypeExpr(QExpression expr, QueryMachine qm, RealWalkerBase walker)
        {
            return expr switch
            {
                QCast f => new SingleTypeCastExpression(f, walker, qm),
                _ => throw new Exception($"Typed expression {expr} not supported")
            };
        }
    }
}