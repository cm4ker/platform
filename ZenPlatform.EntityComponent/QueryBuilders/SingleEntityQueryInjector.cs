using System;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
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
        public void InjectDataSource(QueryMachine qm, IType t,
            IQueryModelContext logicalTreeNode)
        {
            var set = t ?? throw new Exception($"This component can't host next type: {t.GetType()}");
            qm.ld_table(set.Metadata.RelTableName);
        }
    }
}