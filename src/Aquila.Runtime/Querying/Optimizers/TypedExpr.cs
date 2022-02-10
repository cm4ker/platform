using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public abstract class TypedExpr
    {
        protected RealWalkerBase Rw { get; }
        protected QueryMachine Qm { get; }

        protected TypedExpr(RealWalkerBase rw, QueryMachine qm)
        {
            Rw = rw;
            Qm = qm;
        }
    }
}