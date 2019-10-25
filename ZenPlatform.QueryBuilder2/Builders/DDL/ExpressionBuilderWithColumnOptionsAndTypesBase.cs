using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public abstract class ExpressionBuilderWithColumnOptionsAndTypesBase<TNext> : ExpressionBuilderWithColumnTypesBase<TNext>, IColumnOptionSyntax<TNext>
    {

        public override abstract void SetType(ColumnType columnType);
        public abstract void SetConstraintDefinition(ConstraintDefinition constraint);

        public abstract ColumnDefinition GetCurrentColumn();

        public TNext ForeignKey(string primaryTableName, string primaryColumnName)
        {

            return ForeignKey(null, null, primaryTableName, primaryColumnName);
        }

        public TNext ForeignKey(string foreignKeyName, string primaryTableName, string primaryColumnName)
        {
            return ForeignKey(foreignKeyName, null, primaryTableName, primaryColumnName);
        }

        public TNext ForeignKey(string foreignKeyName, string primaryTableSchema, string primaryTableName, string primaryColumnName)
        {
            var constraint = new ConstraintDefinitionForeignKey()
            {
                ForeignTable = new Table() { Value = primaryTableName },
                Name = foreignKeyName
            };

            constraint.Columns.Add(GetCurrentColumn().Column);
            constraint.ForeignColumns.Add(new Column() { Value = primaryColumnName });

            SetConstraintDefinition(constraint);

            return (TNext)(object)this;
        }

        public TNext Identity()
        {
            throw new NotImplementedException();
        }

        public TNext Indexed()
        {
            throw new NotImplementedException();
        }

        public TNext Indexed(string indexName)
        {
            throw new NotImplementedException();
        }

        public TNext NotNullable()
        {
            GetCurrentColumn().IsNotNull = true;
            return (TNext)(object)this;
        }

        public TNext Nullable()
        {
            GetCurrentColumn().IsNotNull = false;
            return (TNext)(object)this;
        }

        public TNext PrimaryKey()
        {
            return PrimaryKey("");
        }

        public TNext PrimaryKey(string primaryKeyName)
        {
            var constraint = new ConstraintDefinitionPrimaryKey()
            {
                Name = primaryKeyName
            };

            constraint.Columns.Add(GetCurrentColumn().Column);

            SetConstraintDefinition(constraint);

            return (TNext)(object)this;
        }

        

        public TNext Unique()
        {
            return Unique("");
        }

        public TNext Unique(string indexName)
        {
            var constraint = new ConstraintDefinitionUnique()
            {
                Name = indexName
            };

            constraint.Columns.Add(GetCurrentColumn().Column);

            SetConstraintDefinition(constraint);

            return (TNext)(object)this;
        }

        public TNext WithColumnDescription(string description)
        {
            throw new NotImplementedException();
        }

        public TNext WithDefault(SystemMethods method)
        {
            GetCurrentColumn().DefaultMethod = method;

            return (TNext)(object)this;
        }

        public TNext WithDefaultValue(object value)
        {
            GetCurrentColumn().DefaultValue = value;

            return (TNext)(object)this;
        }
    }
}
