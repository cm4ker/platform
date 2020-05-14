using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.Factoryes;
using Aquila.QueryBuilder.DML.OrderBy;

namespace Aquila.QueryBuilder.DML.Select
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
