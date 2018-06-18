using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DML.Where
{
    public class WhereExpression : SqlNode
    {
        public bool IsNot { get; set; }
    }
}