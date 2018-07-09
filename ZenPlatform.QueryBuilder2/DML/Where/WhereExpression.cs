using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class WhereExpression : Node
    {
        public bool IsNot { get; set; }
    }
}