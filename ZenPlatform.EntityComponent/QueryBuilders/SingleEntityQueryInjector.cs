using System;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.QueryBuilders
{
    public class SingleEntityQueryInjector : IQueryInjector
    {
        private readonly IComponent _component;

        public SingleEntityQueryInjector(IComponent component)
        {
            _component = component;
        }

        /// <inheritdoc />
        public void InjectTypeSource(QueryMachine qm, IPType t,
            IQueryModelContext logicalTreeNode)
        {
            var set = t ?? throw new ArgumentNullException();
            qm.ld_table(set.GetSettings().DatabaseName);
        }

        public void InjectTableSource(QueryMachine qm, ITable t, IQueryModelContext logicalTreeNode)
        {
            var set = t ?? throw new ArgumentNullException();
            qm.ld_table(set.GetSettings().DatabaseName);
        }
    }
}