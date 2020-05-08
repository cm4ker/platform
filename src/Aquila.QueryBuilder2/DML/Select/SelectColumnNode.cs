using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
{
    public class SelectColumnNode : SqlNode
    {
        public SelectColumnNode(string fieldName)
        {
            Childs.Add(new IdentifierNode(fieldName));
        }

        public SelectColumnNode(string fieldName, string alias) : this(fieldName)
        {
            this.As(alias);
        }

        public SelectColumnNode(string tableName, string fieldName, string alias) : this(fieldName, alias)
        {
            this.WithTableName(tableName);
        }

        public SelectColumnNode As(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                Childs.Add(new AliasNode(alias));

            return this;
        }

        public SelectColumnNode WithTableName(string tableName)
        {
            Childs.Insert(0, new IdentifierNode(tableName));
            Childs.Insert(1, Tokens.SchemaSeparator);

            return this;
        }
    }
}