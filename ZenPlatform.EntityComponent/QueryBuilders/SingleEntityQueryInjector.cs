using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
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
        public string GetDataSourceFragment(SelectBuilder builder, XCObjectTypeBase t,
            IQueryModelContext logicalTreeNode)
        {
            var set = t as XCSingleEntity ?? throw new Exception($"This component can't host next type: {t.GetType()}");

            builder.From(set.RelTableName);

            return set.RelTableName;
        }
    }
}