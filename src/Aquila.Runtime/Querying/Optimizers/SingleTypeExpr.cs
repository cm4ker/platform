using System;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public class SingleTypeExpr : TypedExpr
    {
        public SingleTypeExpr(SelectionRealWalker rw, QueryMachine qm) : base(rw, qm)
        {
        }

        public virtual void Emit()
        {
            throw new NotImplementedException();
        }
    }
}