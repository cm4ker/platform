using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ZenPlatform.QueryBuilder2
{
    public class SelectQueryNode : SqlNode
    {
        private SelectNode _select;
        private WhereNode _where;
        private HavingNode _having;
        private GroupByNode _groupBy;
        private FromNode _from;

        public SelectQueryNode()
        {
            _select = new SelectNode();
            _where = new WhereNode();
            _having = new HavingNode();
            _groupBy = new GroupByNode();
            _from = new FromNode();

            Childs.AddRange(new SqlNode[] {_select, _from, _where, _groupBy, _having});
        }

        public SelectQueryNode Select(string fieldName)
        {
            _select.Add(new SelectFieldNode(fieldName));
            return this;
        }

        public SelectQueryNode Select(string fieldName, string alias)
        {
            _select.Add(new SelectFieldNode(fieldName, alias));
            return this;
        }


        public SelectQueryNode SelectRaw(string raw)
        {
            _select.Add(new RawSqlNode(raw));
            return this;
        }

        public SelectQueryNode Select(string fieldName, string tableAliasOrName, string alias)
        {
            var tableNode = _from.Childs.FirstOrDefault(x =>
            {
                // Это вообще неуниверсальный механизм, так как бывают базы данных с
                // регистро-зависимыми именами пример: postgres
                if (x is IAliasedNode an && an.IsAliased && an.Alias == tableAliasOrName)
                    return true;
                if (x is TableNode table && !table.IsAliased && table.TableName == tableAliasOrName)
                    return true;
                return false;
            });

            if (tableNode is null)
                throw new Exception("Unknow table");

            _select.Add(new SelectFieldNode(fieldName, alias, tableNode));
            return this;
        }

        public SelectQueryNode Select(string fieldName, string tableSchema, string tableName, string alias)
        {
            var tableNode = _from.Childs.FirstOrDefault(x =>
            {
                // Это вообще неуниверсальный механизм, так как бывают базы данных с
                // регистро-зависимыми именами пример: postgres
                if (x is TableNode table && !table.IsAliased && table.TableName == tableName &&
                    table.Schema == tableSchema)
                    return true;
                return false;
            });

            if (tableNode is null)
                throw new Exception("Unknow table");

            _select.Add(new SelectFieldNode(fieldName, alias));
            return this;
        }

        public SelectQueryNode From(string tableName, string alias = "")
        {
            _from.Add(new TableNode("", tableName, alias));
            return this;
        }

        public SelectQueryNode From(SelectQueryNode queryNode, string alias)
        {
            _from.Add(new SelectNastedQueryNode(queryNode, alias));
            return this;
        }

        public SelectQueryNode FromWithSchema(string tableSchema, string tableName, string alias = "")
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            _from.Add(new TableNode(tableSchema, tableName, alias));
            return this;
        }

        public SelectQueryNode Join(JoinType joinType, string tableName, string alias = "")
        {
            _from.Add(new JoinNode(new TableNode(tableName, alias), joinType));
            return this;
        }

        public SelectQueryNode Join(JoinType joinType, SelectQueryNode queryNode, string alias)
        {
            _from.Add(new JoinNode(new SelectNastedQueryNode(queryNode, alias), joinType));
            return this;
        }

        public SelectQueryNode InnerJoin(string tableName, string alias = "")
        {
            return Join(JoinType.Inner, tableName, alias);
        }

        public SelectQueryNode InnerJoin(SelectQueryNode queryNode, string alias)
        {
            return Join(JoinType.Inner, queryNode, alias);
        }

        public SelectQueryNode LeftJoin(string tableName, string alias = "")
        {
            return Join(JoinType.Left, tableName, alias);
        }

        public SelectQueryNode LeftJoin(SelectQueryNode queryNode, string alias)
        {
            return Join(JoinType.Left, queryNode, alias);
        }

        public SelectQueryNode Where(string field1, string operation, string field2)
        {
            _where.Add(new BinaryWhereNode(field1, operation, field2));
            return this;
        }

        public SelectQueryNode WhereIsNull(string fieldName)
        {
            _where.Add(new IsNullWhereNode(fieldName));
            return this;
        }

        public SelectQueryNode WhereLike(string fieldName, string pattern)
        {
            _where.Add(new LikeWhereNode(new FieldNode(fieldName), new StringLiteralNode(pattern)));
            return this;
        }

        public SelectQueryNode WhereIn()
        {
            _where.Add(new InWhereNode());
            return this;
        }
    }

    public class FieldNode : SqlNode
    {
        public FieldNode(string fieldName)
        {
        }
    }

    public class RawSqlNode : SqlNode
    {
        public RawSqlNode(string raw)
        {
            Raw = raw;
        }

        public string Raw { get; set; }
    }

    public class StringLiteralNode : SqlNode
    {
        public string RawString { get; }

        public StringLiteralNode(string rawString)
        {
            RawString = rawString;
        }
    }


    public class IsNullWhereNode : WhereNode
    {
        public IsNullWhereNode(string fieldName)
        {
        }
    }
}