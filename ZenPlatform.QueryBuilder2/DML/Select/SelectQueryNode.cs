﻿using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.GroupBy;
using ZenPlatform.QueryBuilder2.DML.Having;
using ZenPlatform.QueryBuilder2.DML.Where;

using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public partial class SelectQueryNode : SqlNode, ISelectQuery
    {
        private SelectNode _select;
        private WhereNode _where;
        private HavingNode _having;
        private GroupByNode _groupBy;
        private FromNode _from;
        private SqlNode _whereNode;
        private SqlNode _fromNode;
        private SqlNode _groupByNode;
        private SqlNode _havingNode;
        private SqlNode _selectNode;


        public SelectQueryNode()
        {
            _select = new SelectNode();
            _where = new WhereNode();
            _having = new HavingNode();
            _groupBy = new GroupByNode();
            _from = new FromNode();

            Childs.AddRange(new SqlNode[] {_select, _from, _where, _groupBy, _having});
        }

        public SelectQueryNode WithTop(int count)
        {
            _select.WithTop(count);
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

        #endregion

        #region ISelectQuery

        SqlNode ISelectQuery.WhereNode => _whereNode;

        SqlNode ISelectQuery.FromNode => _fromNode;

        SqlNode ISelectQuery.GroupByNode => _groupByNode;

        SqlNode ISelectQuery.HavingNode => _havingNode;

        SqlNode ISelectQuery.SelectNode => _selectNode;

        #endregion
    }


    public interface ISelectQuery : IChildItem<SqlNode>, IParentItem<SqlNode, SqlNode>
    {
        SqlNode WhereNode { get; }
        SqlNode FromNode { get; }
        SqlNode GroupByNode { get; }
        SqlNode HavingNode { get; }
        SqlNode SelectNode { get; }
    }
}