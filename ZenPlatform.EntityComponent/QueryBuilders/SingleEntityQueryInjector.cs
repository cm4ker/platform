using System;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.QueryBuilders
{
    public class SingleEntityQueryInjector : IQueryInjector
    {
        private readonly XCComponent _component;

        public SingleEntityQueryInjector(XCComponent component)
        {
            _component = component;
        }

        /// <inheritdoc />
        public void InjectDataSource(QueryMachine qm, XCObjectTypeBase t,
            IQueryModelContext logicalTreeNode)
        {
            var set = t as XCSingleEntity ?? throw new Exception($"This component can't host next type: {t.GetType()}");
            qm.ld_table(set.RelTableName);
        }
    }
}