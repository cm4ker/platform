using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class UpdateBuilder
    {
        private UpdateNode _updataNode;

        public UpdateBuilder(UpdateNode updataNode)
        {
            _updataNode = updataNode;

            
        }

        public UpdateBuilder Update(string tableName)
        {
            _updataNode.Update = new Table() { Value = tableName };
            return this;
        }

        public UpdateBuilder Set(string fieldName, object value)
        {
            Set(fieldName, v => v.Const(value));
            return this;
        }

        public UpdateBuilder Set(string fieldName, Action<ExpressionBuilder> valueBuilder)
        {
            var builder = new ExpressionBuilder();
            valueBuilder(builder);
            _updataNode.Set.Add(new SetNode()
            {
                Field = new TableFieldNode() { Field = fieldName },
                Value = builder.Expression
            });
            return this;
        }

        public UpdateBuilder Set(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (var s in values)
            {
                Set(s.Key, s.Value);
            }
            return this;
        }

        public UpdateBuilder Where(Action<ExpressionBuilder> conditions)
        {
            _updataNode.Where = new WhereNode();

            ExpressionBuilder builder = new ExpressionBuilder();
            conditions(builder);
            _updataNode.Where.Condition = (ConditionNode)builder.Expression;
            return this;
        }


    }
}
