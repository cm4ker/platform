using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{

    public interface AliasedBuilder<TNext>
    {
        TNext As(string alias);
    }

    public class SelectBuilder: AliasedBuilder<SelectBuilder>
    {
        private SelectNode _selectNode;

        public SelectBuilder(SelectNode selectNode)
        {
            _selectNode = selectNode;
        }

        public SelectBuilder Select(Action<SelectFieldsBuilder> action)
        {

            var builder = new SelectFieldsBuilder(_selectNode.Fields);

            action(builder);

            return this;
        }


        public SelectBuilder SelectField(string fieldName)
        {
            _selectNode.Fields.Add(new TableFieldNode() { Field = fieldName });
            return this;
        }

        public SelectBuilder SelectAll()
        {
            _selectNode.Fields.Add(new AllFieldNode());
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

        public AliasedBuilder<SelectBuilder> From(Action<SelectBuilder> subSelectBuilder)
        {

            var subSelectNode = new SelectNode();
            var builder = new SelectBuilder(subSelectNode);
            subSelectBuilder(builder);

            _selectNode.From = new FromNode();
            _selectNode.From.DataSource = subSelectNode;
            return this;
        }

        public SelectBuilder As(string alias)
        {
            _selectNode.From.DataSource = new DataSourceAliasedNode() { Node = _selectNode.From.DataSource, Alias = alias };
            return this;
        }

        public SelectBuilder Join(string tableName, Action<ExpressionBuilder> conditions, JoinType joinType)
        {
            ExpressionBuilder builder = new ExpressionBuilder();
            conditions(builder);
            _selectNode.From.Join.Add(new JoinNode()
            {
                Condition = (ConditionNode)builder.Expression,
                DataSource = new TableSourceNode() { Table = new Table() { Value = tableName} },
                JoinType = joinType
            }) ;

            return this;
        }

        public SelectBuilder LeftJoin(string tableName, Action<ExpressionBuilder> conditions)
        {
            return Join(tableName, conditions, JoinType.Left);
        }

        public SelectBuilder InnerJoin(string tableName, Action<ExpressionBuilder> conditions)
        {
            return Join(tableName, conditions, JoinType.Inner);
        }

        public SelectBuilder RightJoin(string tableName, Action<ExpressionBuilder> conditions)
        {
            return Join(tableName, conditions, JoinType.Right);
        }



        public SelectBuilder Where(Action<ExpressionBuilder> conditions)
        {
            _selectNode.Where = new WhereNode();

            ExpressionBuilder builder = new ExpressionBuilder();
            conditions(builder);
            _selectNode.Where.Condition = (ConditionNode)builder.Expression;
            return this;
        }

        public SelectBuilder GroupByField(string fieldName, string tableName = null)
        {

            if (_selectNode.GroupBy == null) _selectNode.GroupBy = new GroupByNode();

            var field = new TableFieldNode() { Field = fieldName };
            if (!string.IsNullOrEmpty(tableName))
                field.Table = new Table() { Value = tableName };

            _selectNode.GroupBy.Fields.Add(field);

            return this;
        }

        public SelectBuilder GroupBy(Action<ExpressionListBuilder> action)
        {

            _selectNode.GroupBy = new GroupByNode();
            var builder = new ExpressionListBuilder(_selectNode.GroupBy.Fields);

            action(builder);

            return this;
        }

        public SelectBuilder OrderBy(Action<OrderByBuilder> action)
        {

            _selectNode.OrderBy = new OrderByNode();
            var builder = new OrderByBuilder(_selectNode.OrderBy);

            action(builder);

            return this;
        }

        public SelectBuilder OrderByField(string fieldName, string tableName = null)
        {

            if (_selectNode.OrderBy == null) _selectNode.OrderBy = new OrderByNode();

            var field = new TableFieldNode() { Field = fieldName };
            if (!string.IsNullOrEmpty(tableName))
                field.Table = new Table() { Value = tableName };

            _selectNode.GroupBy.Fields.Add(field);

            return this;
        }





    }
}
