using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying.Optimizers
{
    public abstract class TypedExpr
    {
        protected RealWalker Rw { get; }
        protected QueryMachine Qm { get; }

        protected TypedExpr(RealWalker rw, QueryMachine qm)
        {
            Rw = rw;
            Qm = qm;
        }
    }
}