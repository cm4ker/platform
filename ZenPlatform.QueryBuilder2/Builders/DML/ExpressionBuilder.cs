using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
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

        public void Equal(Action<ExpressionBuilder> left, Action<ExpressionBuilder> reight)
        {
            var expressionNode = new ConditionEqualNode();


            var leftbuilder = new ExpressionBuilder();
            left(leftbuilder);
            expressionNode.Left = leftbuilder.Expression;

            var reightBuilder = new ExpressionBuilder();
            left(reightBuilder);
            expressionNode.Reight = reightBuilder.Expression;


            _expressionNode = expressionNode;

        }

        public void Equal(string fieldName, object value)
        {
            Equal(left => left.Field(fieldName), reight => reight.Const(value));
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

    }
}
