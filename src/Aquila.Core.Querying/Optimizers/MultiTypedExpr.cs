using System;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public abstract class MultiTypedExpr : TypedExpr
    {
        protected MultiTypedExpr(RealWalker rw, QueryMachine qm) : base(rw, qm)
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

        public virtual void EmitValueColumn(IPType ipType)
        {
            throw new NotImplementedException();
        }
    }
}