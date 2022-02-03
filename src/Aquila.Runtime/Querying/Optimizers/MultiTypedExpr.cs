using System;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public abstract class MultiTypedExpr : TypedExpr
    {
        protected MultiTypedExpr(SelectionRealWalker rw, QueryMachine qm) : base(rw, qm)
        {
        }

        public virtual void EmitTypeColumn()
        {
            throw new NotImplementedException();
        }

        public virtual void EmitRefColumn()
        {
            throw new NotImplementedException();
        }

        public virtual void EmitValueColumn(SMType ipType)
        {
            throw new NotImplementedException();
        }
    }
}