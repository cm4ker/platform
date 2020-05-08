﻿using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Conditions;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Where;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Delete
{
    public class DeleteQueryNode : SqlNode
    {
        private DeleteNode _delete;
        private WhereNode _where;
        private FromNode _from;

        public DeleteQueryNode()
        {
            _delete = new DeleteNode();
            _where = new WhereNode();
            _from = new FromNode();

            Childs.AddRange(new Node[] {_delete, _from, _where});
        }

        public DeleteQueryNode Delete(string tableName)
        {
            Delete(f => f.Table(tableName));
            return this;
        }

        public DeleteQueryNode Delete(Func<SqlNodeFactory, AliasedTableNode> exp)
        {
            var factory = new SqlNodeFactory();
            _delete.Add(exp(factory));
            return this;
        }

        public DeleteQueryNode From(string tableName, Action<AliasedTableNode> tableOptions)
        {
            var table = new AliasedTableNode(tableName);

            tableOptions(table);

            _from.Add(table);
            return this;
        }

        public DeleteQueryNode From(string tableName, string alias = "")
        {
            return From(tableName, (t) => t.As(alias));
        }

        public DeleteQueryNode From(SelectQueryNode queryNode, Action<SelectNastedQueryNode> options)
        {
            var nasted = new SelectNastedQueryNode(queryNode);

            options(nasted);

            _from.Add(nasted);
            return this;
        }

        public DeleteQueryNode From(SelectQueryNode queryNode, string alias)
        {
            return From(queryNode, (o) => o.As(alias));
        }

        public DeleteQueryNode Where(string rawLeft, string operation, string rawRight)
        {
            _where.Add(new BinaryConditionNode(rawLeft, operation, rawRight));
            return this;
        }

        public DeleteQueryNode Where(Func<SqlNodeFactory, SqlNode> f1, string operation, Func<SqlNodeFactory, SqlNode> f2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new BinaryConditionNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public DeleteQueryNode WhereIsNull(Func<SqlNodeFactory, Node> fieldExp)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public DeleteQueryNode WhereLike(Func<SqlNodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new LikeConditionNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public DeleteQueryNode WhereIn(Func<SqlNodeFactory, SqlNode> fieldExp, Func<SqlNodeFactory, SqlNode> fieldExp2)
        {
            var factory = new SqlNodeFactory();
            _where.Add(new InConditionNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }
    }
}