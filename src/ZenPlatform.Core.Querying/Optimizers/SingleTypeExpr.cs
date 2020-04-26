using System;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying.Optimizers
{
    public class SingleTypeExpr : TypedExpr
    {
        public SingleTypeExpr(RealWalker rw, QueryMachine qm) : base(rw, qm)
        {
        }

        public virtual void Emit()
        {
            throw new NotImplementedException();
        }
    }
}