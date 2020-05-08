using System;
using System.Collections.Generic;
using Aquila.QueryBuilder.Common;

namespace Aquila.QueryBuilder.DML.OrderBy
{
    public class OrderByNode: SqlNode
    {
        public bool IsDesc { get; set; } = false;
    }
}
