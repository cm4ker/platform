using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.QueryBuilder2.DML.Where;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Delete
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

            Childs.AddRange(new SqlNode[] {_delete, _from, _where});
        }

        public DeleteQueryNode Delete(string tableName)
        {
            Delete(f => f.Table(tableName));
            return this;
        }

        public DeleteQueryNode Delete(Func<NodeFactory, AliasedTableNode> exp)
        {
            var factory = new NodeFactory();
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
            _where.Add(new BinaryWhereNode(rawLeft, operation, rawRight));
            return this;
        }

        public DeleteQueryNode Where(Func<NodeFactory, SqlNode> f1, string operation, Func<NodeFactory, SqlNode> f2)
        {
            var factory = new NodeFactory();
            _where.Add(new BinaryWhereNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public DeleteQueryNode WhereIsNull(Func<NodeFactory, SqlNode> fieldExp)
        {
            var factory = new NodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public DeleteQueryNode WhereLike(Func<NodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new NodeFactory();
            _where.Add(new LikeWhereNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public DeleteQueryNode WhereIn(Func<NodeFactory, SqlNode> fieldExp, Func<NodeFactory, SqlNode> fieldExp2)
        {
            var factory = new NodeFactory();
            _where.Add(new InWhereNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }
    }
}