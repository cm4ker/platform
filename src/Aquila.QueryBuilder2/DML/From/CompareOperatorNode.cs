using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.From
{
    public class CompareOperatorNode : SqlNode
    {
        public CompareOperatorNode(string op)
        {
            Childs.Add(new RawSqlNode(op));
        }
    }
}