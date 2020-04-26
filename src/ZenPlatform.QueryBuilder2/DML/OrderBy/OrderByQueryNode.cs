using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.QueryBuilder.DML.OrderBy
{
    public class OrderByQueryNode
    {
        private readonly SelectQueryNode _select;
        private readonly OrderByNode _order;
        public OrderByQueryNode(SelectQueryNode select, OrderByNode order)
        {
            _select = select;
            _order = order;
        }

        public SelectQueryNode Desc()
        {
            _order.IsDesc = true;
            return _select;
        }

        public SelectQueryNode Asc()
        {
            _order.IsDesc = false;
            return _select;
        }
    }
}
