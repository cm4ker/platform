using System;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.GroupBy;
using ZenPlatform.QueryBuilder.DML.Having;
using ZenPlatform.QueryBuilder.DML.Where;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public partial class SelectQueryNode : SqlNode, ISelectQuery
    {
        private SelectNode _select;
        private WhereNode _where;
        private HavingNode _having;
        private GroupByNode _groupBy;
        private FromNode _from;
        private TopNode _top;

        public SelectQueryNode()
        {
            _select = new SelectNode();
            _where = new WhereNode();
            _having = new HavingNode();
            _groupBy = new GroupByNode();
            _from = new FromNode();
        }

        public SelectQueryNode WithTop(int count)
        {
            _top = new TopNode(count);

            return this;
        }

        #region Select

        public SelectQueryNode Select(string fieldName)
        {
            _select.Select(fieldName);
            return this;
        }

        public SelectQueryNode Select(string fieldName, string alias)
        {
            _select.Select(fieldName, alias);
            return this;
        }

        public SelectQueryNode SelectRaw(string raw)
        {
            _select.SelectRaw(raw);
            return this;
        }

        public SelectQueryNode Select(string tableName, string fieldName, string alias)
        {
            _select.Select(tableName, fieldName, alias);
            return this;
        }

        #endregion

        #region From

        public SelectQueryNode From(string tableName, Action<AliasedTableNode> tableOptions)
        {
            var table = new AliasedTableNode(tableName);

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

        #endregion

        #region Where

        public SelectQueryNode Where(string rawLeft, string operation, string rawRight)
        {
            _where.Add(new BinaryWhereNode(rawLeft, operation, rawRight));
            return this;
        }

        public SelectQueryNode Where(Func<SqlNodeFactory, SqlNode> f1, string operation,
            Func<SqlNodeFactory, SqlNode> f2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new BinaryWhereNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public SelectQueryNode WhereIsNull(Func<SqlNodeFactory, SqlNode> fieldExp)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public SelectQueryNode WhereLike(Func<SqlNodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new LikeWhereNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public SelectQueryNode WhereIn(Func<SqlNodeFactory, SqlNode> fieldExp, Func<SqlNodeFactory, SqlNode> fieldExp2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new InWhereNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }

        #endregion

        #region ISelectQuery

        WhereNode ISelectQuery.WhereNode => _where;

        FromNode ISelectQuery.FromNode => _from;

        GroupByNode ISelectQuery.GroupByNode => _groupBy;

        HavingNode ISelectQuery.HavingNode => _having;

        SelectNode ISelectQuery.SelectNode => _select;

        TopNode ISelectQuery.TopNode => _top;

        #endregion
    }


    /// <summary>
    /// Интерфейс для доступа к частям инструкции SELECT
    /// </summary>
    public interface ISelectQuery : IChildItem<Node>, IParentItem<Node, Node>
    {
        WhereNode WhereNode { get; }
        FromNode FromNode { get; }
        GroupByNode GroupByNode { get; }
        HavingNode HavingNode { get; }
        SelectNode SelectNode { get; }
        TopNode TopNode { get; }
    }
}