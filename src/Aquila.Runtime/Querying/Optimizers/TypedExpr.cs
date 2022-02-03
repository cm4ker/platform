using Aquila.QueryBuilder;

namespace Aquila.Core.Querying.Optimizers
{
    public abstract class TypedExpr
    {
        protected SelectionRealWalker Rw { get; }
        protected QueryMachine Qm { get; }

        protected TypedExpr(SelectionRealWalker rw, QueryMachine qm)
        {
            Rw = rw;
            Qm = qm;
        }
    }
}