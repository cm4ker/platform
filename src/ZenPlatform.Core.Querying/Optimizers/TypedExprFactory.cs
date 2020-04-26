using System;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying.Optimizers
{
    public class TypedExprFactory
    {
        public static MultiTypedExpr CreateMultiTypedExpr(QExpression expr, QueryMachine qm, RealWalker walker)
        {
            if (expr is QField f) return new MultiTypeQFieldExpression(f, walker, qm);
            if (expr is QCase @case) return new MultiTypeCaseExpression(@case, walker, qm);
            if (expr is QParameter o) return new MultiTypeQParameterExpression(o, walker, qm);
            
            throw new Exception($"Typed expression {expr} not supported");
        }

        public static SingleTypeExpr CreateSingleTypeExpr(QExpression expr, QueryMachine qm, RealWalker walker)
        {
            if (expr is QCast f) return new SingleTypeCastExpression(f, walker, qm);

            throw new Exception($"Typed expression {expr} not supported");
        }
    }
}