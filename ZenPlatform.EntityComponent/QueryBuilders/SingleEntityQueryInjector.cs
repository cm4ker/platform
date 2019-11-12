using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.DML.Select;

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
        public string GetDataSourceFragment(QueryMachine qm, XCObjectTypeBase t,
            IQueryModelContext logicalTreeNode)
        {
            var set = t as XCSingleEntity ?? throw new Exception($"This component can't host next type: {t.GetType()}");

            qm.ld_table();

            return set.RelTableName;
        }
    }
}