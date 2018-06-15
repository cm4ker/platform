using System;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using ZenPlatform.QueryBuilder2.From;
using ZenPlatform.QueryBuilder2.Where;

namespace ZenPlatform.QueryBuilder2.Select
{
    public partial class SelectQueryNode : SqlNode
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

        public SelectQueryNode Select(string tableName, string fieldName, string alias)
        {
            _select.Add(new SelectFieldNode(tableName, fieldName, alias));
            return this;
        }

        public SelectQueryNode Select(string fieldName, string tableSchema, string tableName, string alias)
        {
            _select.Add(new SelectFieldNode(fieldName, alias));
            return this;
        }

        public SelectQueryNode From(string tableName, Action<TableNode> tableOptions)
        {
            var table = new TableNode(tableName);

            tableOptions(table);

            _from.Add(table);
            return this;
        }

        public SelectQueryNode From(string tableName, string alias = "")
        {
            return From(tableName, (t) => t.As(alias));
        }

        public SelectQueryNode From(SelectQueryNode queryNode, Action<SelectNastedQueryNode> options)
        {
            var nasted = new SelectNastedQueryNode(queryNode);

            options(nasted);

            _from.Add(nasted);
            return this;
        }

        public SelectQueryNode From(SelectQueryNode queryNode, string alias)
        {
            return From(queryNode, (o) => o.As(alias));
        }


        public SelectQueryNode Where(string rawLeft, string operation, string rawRight)
        {
            _where.Add(new BinaryWhereNode(rawLeft, operation, rawRight));
            return this;
        }

        public SelectQueryNode Where(Func<NodeFactory, SqlNode> f1, string operation, Func<NodeFactory, SqlNode> f2)
        {
            var factory = new NodeFactory();
            _where.Add(new BinaryWhereNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public SelectQueryNode WhereIsNull(Func<NodeFactory, SqlNode> fieldExp)
        {
            var factory = new NodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public SelectQueryNode WhereLike(Func<NodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new NodeFactory();
            _where.Add(new LikeWhereNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public SelectQueryNode WhereIn(Func<NodeFactory, SqlNode> fieldExp, Func<NodeFactory, SqlNode> fieldExp2)
        {
            var factory = new NodeFactory();
            _where.Add(new InWhereNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }
    }

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
        public IsNullWhereNode(SqlNode exp)
        {
            Childs.Add(exp);
        }
    }
}