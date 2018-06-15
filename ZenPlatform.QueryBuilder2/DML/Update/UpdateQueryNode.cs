using System;
using ZenPlatform.QueryBuilder2.From;
using ZenPlatform.QueryBuilder2.Select;
using ZenPlatform.QueryBuilder2.Where;

namespace ZenPlatform.QueryBuilder2.Update
{
    public class UpdateQueryNode : SqlNode
    {
        private WhereNode _where;
        private FromNode _from;
        private UpdateNode _update;
        private SetNode _set;

        public UpdateQueryNode()
        {
            _where = new WhereNode();
            _from = new FromNode();
            _update = new UpdateNode();
            _set = new SetNode();
            Childs.AddRange(new SqlNode[] {_update, _set, _from, _where});
        }

        public UpdateQueryNode Update(string tableName)
        {
            Update(f => f.Table(tableName));
            return this;
        }

        public UpdateQueryNode Update(Func<NodeFactory, TableNode> exp)
        {
            var factory = new NodeFactory();
            _update.Add(exp(factory));
            return this;
        }

        public UpdateQueryNode Set(string fieldName, string stringExpression)
        {
            Set(f => f.Field(fieldName), v => v.String(stringExpression));
            return this;
        }

        public UpdateQueryNode Set(string tableName, string fieldName, string stringExpression)
        {
            Set(f => f.Field(fieldName).WithParent(tableName), v => v.String(stringExpression));
            return this;
        }

        public UpdateQueryNode Set(Func<NodeFactory, FieldNode> exp, Func<NodeFactory, SqlNode> valueExp)
        {
            var fact = new NodeFactory();

            var set = new SetFieldNode(exp(fact), valueExp(fact));
            _set.Add(set);

            return this;
        }

        public UpdateQueryNode From(string tableName, Action<TableNode> tableOptions)
        {
            var table = new TableNode(tableName);

            tableOptions(table);

            _from.Add(table);
            return this;
        }

        public UpdateQueryNode From(string tableName, string alias = "")
        {
            return From(tableName, (t) => t.As(alias));
        }

        public UpdateQueryNode From(SelectQueryNode queryNode, Action<SelectNastedQueryNode> options)
        {
            var nasted = new SelectNastedQueryNode(queryNode);

            options(nasted);

            _from.Add(nasted);
            return this;
        }

        public UpdateQueryNode From(SelectQueryNode queryNode, string alias)
        {
            return From(queryNode, (o) => o.As(alias));
        }

        public UpdateQueryNode Where(string rawLeft, string operation, string rawRight)
        {
            _where.Add(new BinaryWhereNode(rawLeft, operation, rawRight));
            return this;
        }

        public UpdateQueryNode Where(Func<NodeFactory, SqlNode> f1, string operation, Func<NodeFactory, SqlNode> f2)
        {
            var factory = new NodeFactory();
            _where.Add(new BinaryWhereNode(f1(factory), operation, f2(factory)));
            return this;
        }

        public UpdateQueryNode WhereIsNull(Func<NodeFactory, SqlNode> fieldExp)
        {
            var factory = new NodeFactory();
            _where.Add(new IsNullWhereNode(fieldExp(factory)));
            return this;
        }

        public UpdateQueryNode WhereLike(Func<NodeFactory, SqlNode> fieldExp, string pattern)
        {
            var factory = new NodeFactory();
            _where.Add(new LikeWhereNode(fieldExp(factory), new StringLiteralNode(pattern)));
            return this;
        }

        public UpdateQueryNode WhereIn(Func<NodeFactory, SqlNode> fieldExp, Func<NodeFactory, SqlNode> fieldExp2)
        {
            var factory = new NodeFactory();
            _where.Add(new InWhereNode(fieldExp(factory), fieldExp2(factory)));
            return this;
        }
    }

    public class SetNode : SqlNode
    {
    }

    public class UpdateNode : SqlNode
    {
    }


    public class SetFieldNode : SqlNode
    {
        public SetFieldNode(FieldNode fieldExpression, SqlNode value)
        {
            Childs.AddRange(new[] {fieldExpression, new CompareOperatorNode("="), value});
        }
    }
}