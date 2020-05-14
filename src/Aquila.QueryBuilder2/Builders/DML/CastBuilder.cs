using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class CastBuilder : ExpressionBuilderWithColumnTypesBase<CastBuilder>
    {
        private CastNode _node;

        public CastBuilder(CastNode node)
        {
            _node = node;
        }
        public override void SetType(ColumnType columnType)
        {
            _node.Type = columnType;
        }
    }
}
