using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class FieldNode : SqlNode
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