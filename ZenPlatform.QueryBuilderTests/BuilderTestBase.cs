using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Tests
{
    public class BuilderTestBase
    {
        public CreateTable GetCreateTable()
        {
            var createTable = new CreateTable();
            createTable.Table = new Table() {Value = "MyTable"};
            createTable.Scheme = new Scheme() {Value = "MyScheme"};
            createTable.Database = new Database() {Value = "MyDatabase"};


            var column1 = new ColumnDefinition()
            {
                Column = new Column() {Value = "Column1"},
                Type = new ColumnTypeInt(),
                IsNotNull = true
            };
            createTable.Columns.Add(column1);
            var pk = new ConstraintDefinitionPrimaryKey();
            pk.Columns.Add(column1.Column);
            createTable.Constraints.Add(pk);


            var column2 = new ColumnDefinition()
            {
                Column = new Column() {Value = "Column2"},
                Type = new ColumnTypeInt(),
                DefaultValue = 10
            };
            createTable.Columns.Add(column2);


            var u = new ConstraintDefinitionUnique();
            u.Columns.Add(column2.Column);
            createTable.Constraints.Add(u);

            var fk = new ConstraintDefinitionForeignKey();
            fk.Columns.Add(column2.Column);
            fk.ForeignColumns.Add(new Column() {Value = "ForeignColumn"});
            fk.ForeignTable = new Table() {Value = "ForeignTable"};
            createTable.Constraints.Add(fk);


            return createTable;


            var nodes = DDLQuery.New()
                .Create().Table("MyTable")
                .WithColumn("Column1").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("Column2").AsInt32().WithDefaultValue(10).Unique()
                .ForeignKey("ForeignTable", "ForeignColumn")
                .Expression;
        }

        public AlterColumn GetAlterColumn()
        {
            var column = new ColumnDefinition()
            {
                Column = new Column() {Value = "Column1"},
                Type = new ColumnTypeInt(),
                IsNotNull = true
            };


            var alterColumn = new AlterColumn()
            {
                Table = new Table() {Value = "MyTable"},
                Column = column
            };

            return alterColumn;
        }


        public DropColumn GetDeleteColumn()
        {
            var deleteColimn = new DropColumn()
            {
                Column = new Column() {Value = "Column1"},
                Table = new Table() {Value = "MyTable"}
            };

            return deleteColimn;
        }


        public ConstraintDefinition GetAddUniqueConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionUnique();
            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }

        public ConstraintDefinition GetAddUniqueConstraintWithName()
        {
            var constraint = new ConstraintDefinitionUnique() {Name = "u_column1_column2"};
            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }

        public ConstraintDefinition GetAddPrimaryKeyConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionPrimaryKey();
            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }

        public ConstraintDefinition GetAddPrimaryKeyConstraintWithName()
        {
            var constraint = new ConstraintDefinitionPrimaryKey() {Name = "pk_column1_column2"};
            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }

        public ConstraintDefinition GetAddForeignKeyConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionForeignKey()
            {
                Name = "fk_column1_column2",
                ForeignTable = new Table() {Value = "ForeignTable"}
            };

            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});
            constraint.ForeignColumns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }

        public ConstraintDefinition GetAddForeignKeyConstraintWithName()
        {
            var constraint = new ConstraintDefinitionForeignKey()
            {
                Name = "fk_column1_column2",
                ForeignTable = new Table() {Value = "ForeignTable"}
            };
            constraint.Columns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});
            constraint.ForeignColumns.AddRange(new List<Column>()
                {new Column() {Value = "Column1"}, new Column() {Value = "Column2"}});

            var addConstraint = new AddConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = constraint
            };

            return constraint;
        }


        public DropConstraint GetDropConstraint()
        {
            var dropConstraint = new DropConstraint()
            {
                Table = new Table() {Value = "MyTable"},
                Constraint = new QueryBuilder.Model.Constraint() {Value = "constraint_name"}
            };

            return dropConstraint;
        }

        public WhereNode GetWhere()
        {
            var where = new WhereNode();
            where.Condition = new ConditionEqualNode()
            {
                Left = new TableFieldNode() {Field = "Field1", Table = new Table() {Value = "MyTable1"}},
                Right = new ConstNode() {Value = 10}
            };

            return where;
        }


        public SelectNode GetSelect()
        {
            var select = new SelectNode();


            select.Fields.Add(new TableFieldNode() {Field = "Field1"});
            select.Fields.Add(new TableFieldNode() {Field = "Field2"});
            select.Fields.Add(new AggregateSumNode() {Node = new TableFieldNode() {Field = "Field3"}});

            select.From = new FromNode();
            var subSelect = new SelectNode()
            {
                From = new FromNode()
                {
                    DataSource = new TableSourceNode() {Table = new Table() {Value = "MySubTable1"}}
                }
            };
            subSelect.Fields.Add(new AllFieldNode());
            select.From.DataSource = new DataSourceAliasedNode()
            {
                Node = subSelect,
                Alias = "MyTable1"
            };
            select.From.Join.Add(new JoinNode()
            {
                JoinType = JoinType.Left,
                DataSource = new TableSourceNode() {Table = new Table() {Value = "Mytable2"}},
                Condition = new ConditionEqualNode()
                {
                    Left = new TableFieldNode() {Field = "Field1", Table = new Table() {Value = "MyTable1"}},
                    Right = new TableFieldNode()
                    {
                        Field = "Field2",
                        Table = new Table() {Value = "MyTable2"},
                    }
                }
            });

            select.Where = GetWhere();


            select.GroupBy = new GroupByNode();
            select.GroupBy.Fields.Add(new TableFieldNode() {Field = "Field1"});
            select.GroupBy.Fields.Add(new TableFieldNode() {Field = "Field2"});

            select.OrderBy = new OrderByNode();
            select.OrderBy.Fields.Add(new TableFieldNode() {Field = "Field1"});
            select.OrderBy.Fields.Add(new TableFieldNode() {Field = "Field2"});
            select.OrderBy.Direction = OrderDirection.DESC;

            return select;
        }


        public InsertNode GetInsert()
        {
            var datasource = new ValuesSourceNode();
            datasource.Values.Add(new ConstNode() {Value = 20});
            datasource.Values.Add(new ConstNode() {Value = 20});
            datasource.Values.Add(new ConstNode() {Value = 20});
            datasource.Values.Add(new ConstNode() {Value = 20});

            var insert = new InsertNode()
            {
                Into = new Table() {Value = "MyTable1"},
                DataSource = datasource
            };


            return insert;
        }


        public UpdateNode GetUpdate()
        {
            var update = new UpdateNode()
            {
                Update = new Table() {Value = "MyTable1"},
            };
            update.Set.Add(new SetNode()
            {
                Field = new TableFieldNode() {Field = "MyField1"},
                Value = new ConstNode() {Value = 10}
            });
            update.Set.Add(new SetNode()
            {
                Field = new TableFieldNode() {Field = "MyField2"},
                Value = new ConstNode() {Value = 10}
            });

            update.Where = GetWhere();

            return update;
        }


        public DeleteNode GetDelete()
        {
            var delete = new DeleteNode();

            delete.From = new Table() {Value = "MyTable1"};
            delete.Where = GetWhere();

            return delete;
        }


        public QuerySyntaxNode GetSelectFromBuilder()
        {
            var selectNode = new SelectNode();

            new SelectBuilder(selectNode)
                .Select(b => b
                    .Field("Field1")
                    .Field("Field2")
                    .Sum(f => f.Field("Field3"))
                )
                .From(s => s.SelectAll().From("MySubTable1")) //nestedQuery
                .As("MyTable1")
                .LeftJoin("Mytable2",
                    e => e.Equal(
                        f => f.Field("Field1", "MyTable1"),
                        f => f.Field("Field2", "MyTable2")
                    ))
                .Where(e => e.Equal(
                    f => f.Field("Field1", "MyTable1"),
                    f => f.Const(10)
                ))
                .GroupBy(g => g
                    .Field("Field1")
                    .Field("Field2"))
                .OrderBy(o => o
                    .Field("Field1")
                    .Field("Field2")
                    .Desc());

            return selectNode;
        }
    }
}