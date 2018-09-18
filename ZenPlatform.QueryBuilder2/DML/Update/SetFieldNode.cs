using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Update
{
    public class SetFieldNode : SqlNode
    {
        public SetFieldNode(ColumnNode columnExpression, SqlNode value)
        {
            Childs.AddRange(new[] {columnExpression, new CompareOperatorNode("="), value});
        }
    }
}