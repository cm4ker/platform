using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class FieldNode : Node
    {
        public FieldNode(string fieldName)
        {
            Childs.Add(new IdentifierNode(fieldName));
        }

        public FieldNode WithParent(string parentName)
        {
            Childs.Insert(0, new IdentifierNode(parentName));
            Childs.Insert(1, new SchemaSeparatorNode());

            return this;
        }
    }
}