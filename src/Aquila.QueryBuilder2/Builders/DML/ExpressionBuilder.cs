using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class ExpressionBuilder
    {
        public ExpressionNode Expression => _expressionNode;
        private ExpressionNode _expressionNode;
        public void And(params ExpressionNode[] exp)
        {
            var expressionNode = new ConditionAndNode();
            expressionNode.Nodes.AddRange(exp);
            _expressionNode = expressionNode;
        }
       
        public void And(params Action<ExpressionBuilder>[] builders)
        {
            var expressionNode = new ConditionAndNode();
            expressionNode.Nodes.AddRange(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }));
            _expressionNode = expressionNode;
            //return this;
        }


        public void Or(params ExpressionNode[] exp)
        {
            var expressionNode = new ConditionOrNode();
            expressionNode.Nodes.AddRange(exp);
            _expressionNode = expressionNode;
        }


        public void Or(params Action<ExpressionBuilder>[] builders)
        {
            var expressionNode = new ConditionOrNode();
            expressionNode.Nodes.AddRange(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }));
            _expressionNode = expressionNode;
            //return this;
        }


        public void Not(ExpressionNode exp)
        {
            var expressionNode = new ConditionNotNode();
            expressionNode.Node = exp;
            _expressionNode = expressionNode;
        }


        public void Not(Action<ExpressionBuilder> builder)
        {
            var expressionNode = new ConditionNotNode();


            ExpressionBuilder b = new ExpressionBuilder();

            builder(b);

            expressionNode.Node = b.Expression;

        }

        public void Equal(Action<ExpressionBuilder> left, Action<ExpressionBuilder> right)
        {
            var expressionNode = new ConditionEqualNode();


            var leftbuilder = new ExpressionBuilder();
            left(leftbuilder);
            expressionNode.Left = leftbuilder.Expression;

            var rightBuilder = new ExpressionBuilder();
            right(rightBuilder);
            expressionNode.Right = rightBuilder.Expression;


            _expressionNode = expressionNode;

        }

        public void Equal(string fieldName, object value)
        {
            Equal(left => left.Field(fieldName), right => right.Const(value));
        }

        public void Const(object constant)
        {
            var expressionNode = new ConstNode();
            expressionNode.Value = constant;
            _expressionNode = expressionNode;
        }

        public void Field(string fieldName, string tableName = null)
        {
            var expressionNode = new TableFieldNode();
            expressionNode.Field = fieldName;
            if (!string.IsNullOrEmpty(tableName))
                expressionNode.Table = new Table() { Value = tableName };
            _expressionNode = expressionNode;
        }

        public void Add(params ExpressionNode[] exp)
        {
            var expressionNode = new ExpressionSumNode();
            expressionNode.Expressions.AddRange(exp);
            _expressionNode = expressionNode;
        }


        public void Add(params Action<ExpressionBuilder>[] builders)
        {
            Add(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }).ToArray());
        }


        public void Add(string fieldNameLeft, string fieldNameRight)
        {
            Add(e => e.Field(fieldNameLeft), e => e.Field(fieldNameRight));
        }

        public void Sum(string fieldNameLeft, object valueRight)
        {
            Add(e => e.Field(fieldNameLeft), e => e.Const(valueRight));
        }


        public void Diff(params ExpressionNode[] exp)
        {
            var expressionNode = new ExpressionDiffNode();
            expressionNode.Expressions.AddRange(exp);
            _expressionNode = expressionNode;
        }


        public void Diff(params Action<ExpressionBuilder>[] builders)
        {
            Diff(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }).ToArray());
        }




        public void Diff(string fieldNameLeft, string fieldNameRight)
        {
            Diff(e => e.Field(fieldNameLeft), e => e.Field(fieldNameRight));
        }

        public void Diff(string fieldNameLeft, object valueRight)
        {
            Diff(e => e.Field(fieldNameLeft), e => e.Const(valueRight));
        }

        public void Dev(params ExpressionNode[] exp)
        {
            var expressionNode = new ExpressionDevNode();
            expressionNode.Expressions.AddRange(exp);
            _expressionNode = expressionNode;
        }


        public void Dev(params Action<ExpressionBuilder>[] builders)
        {
            Dev(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }).ToArray());
        }


        public void Dev(string fieldNameLeft, string fieldNameRight)
        {
            Dev(e => e.Field(fieldNameLeft), e => e.Field(fieldNameRight));
        }

        public void Dev(string fieldNameLeft, object valueRight)
        {
            Dev(e => e.Field(fieldNameLeft), e => e.Const(valueRight));
        }

        public void Mul(params ExpressionNode[] exp)
        {
            var expressionNode = new ExpressionMulNode();
            expressionNode.Expressions.AddRange(exp);
            _expressionNode = expressionNode;
        }


        public void Mul(params Action<ExpressionBuilder>[] builders)
        {
            Mul(builders.Select(b =>
            {

                ExpressionBuilder builder = new ExpressionBuilder();

                b(builder);

                return builder.Expression;
            }).ToArray());
        }


        public void Mul(string fieldNameLeft, string fieldNameRight)
        {
            Mul(e => e.Field(fieldNameLeft), e => e.Field(fieldNameRight));
        }

        public void Mul(string fieldNameLeft, object valueRight)
        {
            Mul(e => e.Field(fieldNameLeft), e => e.Const(valueRight));
        }

        public void AggregateSum(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateSumNode();
            expressionNode.Node = builder.Expression;
            _expressionNode = expressionNode;
        }

        public void AggregateCount(Action<ExpressionBuilder> action)
        {
            ExpressionBuilder builder = new ExpressionBuilder();

            action(builder);

            var expressionNode = new AggregateCountNode();
            expressionNode.Node = builder.Expression;
            _expressionNode = expressionNode;
        }

        public void Exists(Action<SelectBuilder> action)
        {
            SelectNode selectNode = new SelectNode();
            SelectBuilder builder = new SelectBuilder(selectNode);

            action(builder);

            _expressionNode = new ExistsNode() { DataSource = selectNode };
        }
    }
}
