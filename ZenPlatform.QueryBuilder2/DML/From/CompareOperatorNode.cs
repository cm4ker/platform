using ZenPlatform.QueryBuilder2.Select;

namespace ZenPlatform.QueryBuilder2.From
{
    public class CompareOperatorNode : SqlNode
    {
        public CompareOperatorNode(string op)
        {
            Childs.Add(new RawSqlNode(op));
        }
    }
}