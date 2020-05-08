using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{



    public class ExpressionListBuilder : ExpressionListBuilderBase<ExpressionListBuilder>
    {
        public ExpressionListBuilder(List<ExpressionNode> list): base(list)
        {
        }
    }

    public class SelectFieldsBuilder : ExpressionListBuilderBase<SelectFieldsBuilder>, IAliasedBuilder<SelectFieldsBuilder>
    {
        public SelectFieldsBuilder(List<ExpressionNode> list) : base(list)
        {
        }

        public SelectFieldsBuilder Cast(Action<CastBuilder> action)
        {
            if (_currentNode == null) return this;

            var node = new CastNode();
            var builder = new CastBuilder(node);
            action(builder);

            node.Expression = _currentNode;

            _list[_list.IndexOf(_currentNode)] = node;

            _currentNode = null;

            return this;

        }

        public SelectFieldsBuilder Sum(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateSumNode();
            expressionNode.Node = builder.Expression;
            _currentNode = expressionNode;
            _list.Add(expressionNode);
            
            return this;
        }


        public SelectFieldsBuilder Count(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateCountNode();
            expressionNode.Node = builder.Expression;
            _currentNode = expressionNode;
            _list.Add(expressionNode);

            return this;
        }

        public SelectFieldsBuilder As(string alias)
        {
            if (_currentNode == null) return this;
            _list[_list.IndexOf(_currentNode)] = new ExpressionAliasedNode() { Alias = alias, Node = _currentNode };

            _currentNode = null;

            return this;
        }
    }

    public class OrderByBuilder : ExpressionListBuilderBase<OrderByBuilder>
    {
        private OrderByNode _orderBy;
        public OrderByBuilder(OrderByNode orderBy) : base(orderBy.Fields)
        {
            _orderBy = orderBy;
        }

        public OrderByBuilder Asc()
        {
            _orderBy.Direction = OrderDirection.ASC;
            return this;
        }

        public OrderByBuilder Desc()
        {
            _orderBy.Direction = OrderDirection.DESC;
            return this;
        }
    }

    public class ExpressionListBuilderBase<TNext>
    {
        protected List<ExpressionNode> _list;
        protected ExpressionNode _currentNode;
        public ExpressionListBuilderBase(List<ExpressionNode> list)
        {
            _list = list;
        }

        public TNext Field(string fieldName, string tableName = null)
        {


            var field = new TableFieldNode() { Field = fieldName };
            if (!string.IsNullOrEmpty(tableName))
                field.Table = new Table() { Value = tableName };
            _currentNode = field;
            _list.Add(field);
            
            return (TNext)(object)this;
        }

        public TNext Exp(Action<ExpressionBuilder> action)
        {
            var builder = new ExpressionBuilder();


            action(builder);
            _currentNode = builder.Expression;
            _list.Add(_currentNode);

            return (TNext)(object)this;
        }
    }
}
