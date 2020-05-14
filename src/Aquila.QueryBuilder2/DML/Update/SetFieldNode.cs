using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.DML.From;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Update
{
    public class SetFieldNode : SqlNode
    {
        public SetFieldNode(ColumnNode columnExpression, SqlNode value)
        {
            Childs.AddRange(new[] {columnExpression, new CompareOperatorNode("="), value});
        }
    }
}