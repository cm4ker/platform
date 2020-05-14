using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.From
{
    public class OnNode : SqlNode
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