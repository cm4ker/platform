using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.From
{
    public class OnNode : Node
    {
        public OnNode(string tableName, string fieldName, string condition, string tableName2, string fieldName2)
        {
            Childs.AddRange(new Node[]
            {
                new IdentifierNode(tableName),
                new SchemaSeparatorNode(),
                new IdentifierNode(fieldName),
                new CompareOperatorNode(condition),
                new IdentifierNode(tableName2),
                new SchemaSeparatorNode(),
                new IdentifierNode(fieldName2),
            });
        }
    }
}