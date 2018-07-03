using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Where
{
    public class WhereExpression : SqlNode
    {
        public bool IsNot { get; set; }
    }
}