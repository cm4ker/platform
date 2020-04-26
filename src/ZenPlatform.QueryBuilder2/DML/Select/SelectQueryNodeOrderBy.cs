using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.DML.OrderBy;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public partial class SelectQueryNode
    {
        public OrderByQueryNode OrderBy(string fieldName)
        {
            return OrderBy(x => x.Field(fieldName));
        }
        public OrderByQueryNode OrderBy(Func<SqlNodeFactory, SqlNode> expression)
        {
            var factory = new SqlNodeFactory();
            _orderBy.Add(expression(factory));

            return new OrderByQueryNode(this, _orderBy);
        }
    }
}
