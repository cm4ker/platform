using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class OnNode : SqlNode
    {
        public OnNode(string tableName, string fieldName, string condition, string tableName2, string fieldName2)
        {
            Childs.AddRange(new SqlNode[]
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