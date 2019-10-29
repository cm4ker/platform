using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{



    public class ExpressionListBuilder : ExpressionListBuilderBase<ExpressionListBuilder>
    {
        public ExpressionListBuilder(List<ExpressionNode> list): base(list)
        {
        }
    }

    public class SelectFieldsBuilder : ExpressionListBuilderBase<SelectFieldsBuilder>
    {
        public SelectFieldsBuilder(List<ExpressionNode> list) : base(list)
        {
        }

        public SelectFieldsBuilder Sum(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateSumNode();
            expressionNode.Node = builder.Expression;
            _list.Add(expressionNode);

            return this;
        }

        public SelectFieldsBuilder Count(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateCountNode();
            expressionNode.Node = builder.Expression;
            _list.Add(expressionNode);

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
        public ExpressionListBuilderBase(List<ExpressionNode> list)
        {
            _list = list;
        }

        public TNext Field(string fieldName, string tableName = null)
        {


            var field = new TableFieldNode() { Field = fieldName };
            if (!string.IsNullOrEmpty(tableName))
                field.Table = new Table() { Value = tableName };

            _list.Add(field);
            
            return (TNext)(object)this;
        }

        public TNext Exp(Action<ExpressionBuilder> action)
        {
            var builder = new ExpressionBuilder();


            action(builder);

            _list.Add(builder.Expression);

            return (TNext)(object)this;
        }
    }
}
