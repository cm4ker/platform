using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.QueryBuilder2.DML.Where;

namespace ZenPlatform.QueryBuilder2
{
    public class NodeFactory
    {
        public FieldNode Field(string name)
        {
            return new FieldNode(name);
        }

        public FieldNode Field(string tableName, string name)
        {
            return (new FieldNode(name)).WithParent(tableName);
        }

        public AliasedTableNode Table(string tableName)
        {
            return new AliasedTableNode(tableName);
        }

        public TableWithColumnsNode InsertTable(string tableName)
        {
            return new TableWithColumnsNode(tableName);
        }

        public AliasedTableNode Table(string schemaName, string tableName)
        {
            return new AliasedTableNode(tableName).WithSchema(schemaName);
        }

        public ParameterNode Parameter(string name)
        {
            return new ParameterNode(name);
        }

        public StringLiteralNode String(string str)
        {
            return new StringLiteralNode(str);
        }
    }
}