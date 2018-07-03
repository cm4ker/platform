using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Update
{
    public class SetFieldNode : SqlNode
    {
        public SetFieldNode(FieldNode fieldExpression, SqlNode value)
        {
            Childs.AddRange(new[] {fieldExpression, new CompareOperatorNode("="), value});
        }
    }
}