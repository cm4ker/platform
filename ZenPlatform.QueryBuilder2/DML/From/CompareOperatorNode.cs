using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class CompareOperatorNode : SqlNode
    {
        public CompareOperatorNode(string op)
        {
            Childs.Add(new RawSqlNode(op));
        }
    }
}