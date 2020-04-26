using System;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.QueryBuilder.DML.OrderBy
{
    public class OrderByNode: SqlNode
    {
        public bool IsDesc { get; set; } = false;
    }
}
