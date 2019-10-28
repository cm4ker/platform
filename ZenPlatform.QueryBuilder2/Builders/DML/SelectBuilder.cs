using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class SelectBuilder
    {
        private SelectNode _selectNode;

        public SelectBuilder(SelectNode selectNode)
        {
            _selectNode = selectNode;
        }

        public SelectBuilder Select(string fieldName)
        {
            _selectNode.Fields.Add(new TableFieldNode() { Field = fieldName });
            return this;
        }

        public SelectBuilder Select(Action<ExpressionBuilder> exp)
        {
            var builder = new ExpressionBuilder();
            exp(builder);
            _selectNode.Fields.Add(builder.Expression);
            return this;
        }

        public SelectBuilder From(string tableName)
        {
            _selectNode.From = new FromNode();
            _selectNode.From.DataSource = new TableSourceNode() { Table = new Table() { Value = tableName } };
            return this;
        }

        public SelectBuilder From(Action<SelectBuilder> subSelectBuilder)
        {

            var subSelectNode = new SelectNode();
            var builder = new SelectBuilder(subSelectNode);
            subSelectBuilder(builder);

            _selectNode.From = new FromNode();
            _selectNode.From.DataSource = subSelectNode;
            return this;
        }

        public SelectBuilder LeftJoin(string tableName, Action<ExpressionBuilder> conditions)
        {
            ExpressionBuilder builder = new ExpressionBuilder();
            conditions(builder);
            _selectNode.From.Join.Add(new JoinNode()
            {
                Condition = (ConditionNode)builder.Expression,
                DataSource = new TableSourceNode() { Table = new Table() { Value = tableName} },
                JoinType = JoinType.Left
            }) ;

            return this;
        }

        public SelectBuilder Where(Action<ExpressionBuilder> conditions)
        {
            _selectNode.Where = new WhereNode();

            ExpressionBuilder builder = new ExpressionBuilder();
            conditions(builder);
            _selectNode.Where.Condition = (ConditionNode)builder.Expression;
            return this;
        }


    }
}
