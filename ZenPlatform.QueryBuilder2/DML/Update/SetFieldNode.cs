using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Update
{
    public class SetFieldNode : Node
    {
        public SetFieldNode(FieldNode fieldExpression, Node value)
        {
            Childs.AddRange(new[] {fieldExpression, new CompareOperatorNode("="), value});
        }
    }
}