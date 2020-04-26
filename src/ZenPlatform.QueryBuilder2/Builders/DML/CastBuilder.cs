using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
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
