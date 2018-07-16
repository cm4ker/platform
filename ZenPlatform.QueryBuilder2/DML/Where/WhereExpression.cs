using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class WhereExpression : SqlNode
    {
        public bool IsNot { get; set; }
    }
}