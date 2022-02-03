using Aquila.QueryBuilder;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public class RealWalkerBase : QLangWalker
    {
        readonly DatabaseRuntimeContext _drContext;
        protected QueryMachine Qm;
        
        public QueryMachine QueryMachine => Qm;
        
        internal DatabaseRuntimeContext DrContext => _drContext;

        /// <summary>
        /// Create instance of real walker class
        /// </summary>
        /// <param name="drContext"></param>
        public RealWalkerBase(DatabaseRuntimeContext drContext)
        {
            _drContext = drContext;
            Qm = new QueryMachine();
        }
    }
}