using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying.Optimizers
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