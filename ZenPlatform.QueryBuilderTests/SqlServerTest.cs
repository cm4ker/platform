using Xunit;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;
using System.Data;
using ZenPlatform.Data;
using System;
using ZenPlatform.QueryBuilder.Visitor;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Tests.SqlBuilder.SqlServer
{
    public class SqlServerTest
    {
        const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private DataContextManager _dataContextManager;
        private DataContext _context;
        private SQLQueryVisitor _visitor = new SQLQueryVisitor();
        public SqlServerTest()
        {
            _dataContextManager = new DataContextManager();
            _dataContextManager.Initialize(SqlDatabaseType.SqlServer, connectionString);

            _context = _dataContextManager.GetContext();
            using (var cmd = _context.CreateCommand())
            {

                cmd.CommandText = "SET PARSEONLY ON";
                cmd.ExecuteScalar();
            }
        }


        private void Check(string script)
        {
            var context = _dataContextManager.GetContext();

            using (var cmd = context.CreateCommand())
            {

                cmd.CommandText = script;

                var msg = string.Empty;
                object res = null;

                try
                {
                    res = cmd.ExecuteScalar();
                } catch (Exception ex)
                {
                    msg = ex.Message;
                }

                Assert.True(string.IsNullOrEmpty(msg), msg);
                

            }



        }



        [Fact]
        public void CreateTable()
        {
            var createTable = new CreateTable();
            createTable.Table = new Table() { Value = "MyTable" };
            createTable.Scheme = new Scheme() { Value = "MyScheme" };
            createTable.Database = new Database() { Value = "MyDatabase" };


            var column1 = new ColumnDefinition()
            {
                Column = new Column() { Value = "Column1" },
                Type = new ColumnTypeInt(),
                IsNotNull = true
            };
            createTable.Columns.Add(column1);
            var pk = new ConstraintDefinitionPrimaryKey();
            pk.Columns.Add(column1.Column);
            createTable.Constraints.Add(pk);


            var column2 = new ColumnDefinition()
            {
                Column = new Column() { Value = "Column2" },
                Type = new ColumnTypeInt(),
                DefaultValue = 10
            };
            createTable.Columns.Add(column2);


            var u = new ConstraintDefinitionUnique();
            u.Columns.Add(column2.Column);
            createTable.Constraints.Add(u);

            var fk = new ConstraintDefinitionForeignKey();
            fk.Columns.Add(column2.Column);
            fk.ForeignColumns.Add(new Column() { Value = "ForeignColumn" });
            fk.ForeignTable = new Table() { Value = "ForeignTable" };
            createTable.Constraints.Add(fk);



            var result = _visitor.Visit(createTable);

            Check(result);




            var nodes = DDLQuery.New()
                .Create().Table("MyTable")
                .WithColumn("Column1").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("Column2").AsInt32().WithDefaultValue(10).Unique().ForeignKey("ForeignTable", "ForeignColumn")
                .Expression;


            var result2 = _visitor.Visit(nodes);

            Assert.Equal(result2, result);


        }

        [Fact]
        public void AlterColumn()
        {

            var column = new ColumnDefinition()
            {
                Column = new Column() { Value = "Column1" },
                Type = new ColumnTypeInt(),
                IsNotNull = true
            };

            
            var alterColumn = new AlterColumn()
            {
                Table = new Table() { Value = "MyTable" },
                Column = column
            };
            var result = _visitor.Visit(alterColumn);

            Check(result);

        }

        [Fact]
        public void DeleteColumn()
        {
            var deleteColimn = new DropColumn()
            {
                Column = new Column() { Value = "Column1" },
                Table = new Table() { Value = "MyTable" }
            };

            var result = _visitor.Visit(deleteColimn);

            Check(result);
        }



        [Fact]
        public void AddUniqueConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionUnique();
            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }

        [Fact]
        public void AddUniqueConstraintWithName()
        {
            var constraint = new ConstraintDefinitionUnique() { Name = "u_column1_column2" };
            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }

        [Fact]
        public void AddPrimaryKeyConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionPrimaryKey();
            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }

        [Fact]
        public void AddPrimaryKeyConstraintWithName()
        {
            var constraint = new ConstraintDefinitionPrimaryKey() { Name = "pk_column1_column2" };
            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }

        [Fact]
        public void AddForeignKeyConstraintWithOutName()
        {
            var constraint = new ConstraintDefinitionForeignKey()
            {
                Name = "fk_column1_column2",
                ForeignTable = new Table() { Value = "ForeignTable" }
            };

            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });
            constraint.ForeignColumns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }

        [Fact]
        public void AddForeignKeyConstraintWithName()
        {
            var constraint = new ConstraintDefinitionForeignKey() {
                Name = "fk_column1_column2",
                ForeignTable = new Table() { Value = "ForeignTable"}
                };
            constraint.Columns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });
            constraint.ForeignColumns.AddRange(new List<Column>() { new Column() { Value = "Column1" }, new Column() { Value = "Column2" } });

            var addConstraint = new AddConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = constraint
            };

            var result = _visitor.Visit(addConstraint);

            Check(result);
        }


        [Fact]
        public void DropConstraint()
        {
            var dropConstraint = new DropConstraint()
            {
                Table = new Table() { Value = "MyTable" },
                Constraint = new QueryBuilder.Model.Constraint() { Value = "constraint_name" }
            };

            var result = _visitor.Visit(dropConstraint);

            Check(result);
        }

        public WhereNode GetWhere()
        {
            var where = new WhereNode();
            where.Condition = new ConditionEqualNode()
            {
                Left = new TableFieldNode() { Field = "Field1", Table = new Table() { Value = "MyTable1" } },
                Right = new ConstNode() { Value = 10 }
            };

            return where;
        }


        public SelectNode GetSelect()
        {
            var select = new SelectNode();


            select.Fields.Add(new TableFieldNode() { Field = "Field1" });
            select.Fields.Add(new TableFieldNode() { Field = "Field2" });
            select.Fields.Add(new AggregateSumNode() { Node = new TableFieldNode() { Field = "Field3" } });

            select.From = new FromNode();
            var subSelect = new SelectNode()
            {
                From = new FromNode()
                {
                    DataSource = new TableSourceNode() { Table = new Table() { Value = "MySubTable1" } }
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
                DataSource = new TableSourceNode() { Table = new Table() { Value = "Mytable2" } },
                Condition = new ConditionEqualNode()
                {
                    Left = new TableFieldNode() { Field = "Field1", Table = new Table() { Value = "MyTable1" } },
                    Right = new TableFieldNode()
                    {
                        Field = "Field2",
                        Table = new Table() { Value = "MyTable2" },
                    }
                }
            });

            select.Where = GetWhere();


            select.GroupBy = new GroupByNode();
            select.GroupBy.Fields.Add(new TableFieldNode() { Field = "Field1" });
            select.GroupBy.Fields.Add(new TableFieldNode() { Field = "Field2" });

            select.OrderBy = new OrderByNode();
            select.OrderBy.Fields.Add(new TableFieldNode() { Field = "Field1" });
            select.OrderBy.Fields.Add(new TableFieldNode() { Field = "Field2" });
            select.OrderBy.Direction = OrderDirection.DESC;

            return select;
        }


        public QuerySyntaxNode GetSelectFromBuilder()
        {
            var query = Query.New();



            query.Select()
                .Select(b => b
                    .Field("Field1")
                    .Field("Field2")
                    .Sum(f=>f.Field("Field3"))
                )
                .From(s=>s.SelectAll().From("MySubTable1")).As("MyTable1")
                .LeftJoin("Mytable2", e => e.Equal(f => f.Field("Field1", "MyTable1"), f => f.Field("Field2", "MyTable2")))
                .Where(e => e.Equal(f => f.Field("Field1", "MyTable1"), f => f.Const(10)))
                .GroupBy(g => g
                    .Field("Field1")
                    .Field("Field2"))
                .OrderBy(o => o
                    .Field("Field1")
                    .Field("Field2")
                    .Desc());
            return query.Expression;
        }

        [Fact]
        public void SelectBuilder()
        {

            Expression s1 = new Expression();
             s1.Nodes.Add(GetSelect());
            var s2 = GetSelectFromBuilder();
            Assert.Equal(s1, s2);
        }


        [Fact]
        public void Select()
        {


            var result = _visitor.Visit(GetSelect());

            Check(result);



            var query = Query.New();

            query.Select()
                .SelectField("Field1")
                .Select(e => e.Sum("Field2", "Field2"))
                .From("dasdasdas")
                .Where(c =>
                {
                    c.And(
                        e => e.Equal("Field1", 10), 
                        e => e.Equal("Field2", 20), 
                        e=>e.Equal(
                            e=>e.Field("sdad"), 
                            e=>e.Sum("Field1","Field2")
                            ));

                });


            

            result = _visitor.Visit(query.Expression);

            Check(result);

        }

        [Fact]
        public void Insert()
        {
            var datasource = new ValuesSourceNode();
            datasource.Values.Add(new ConstNode() { Value = 20 });
            datasource.Values.Add(new ConstNode() { Value = 20 });
            datasource.Values.Add(new ConstNode() { Value = 20 });
            datasource.Values.Add(new ConstNode() { Value = 20 });

            var insert = new InsertNode()
            {
                Into = new Table() { Value = "MyTable1" },
                DataSource = datasource
            };


            var result = _visitor.Visit(insert);

            Check(result);
        }

        [Fact]
        public void Update()
        {

            var update = new UpdateNode()
            {
                Update = new Table() { Value = "MyTable1" },
            };
            update.Set.Add(new SetNode()
            {
                Field = new TableFieldNode() { Field = "MyField1" },
                Value = new ConstNode() { Value = 10 }
            });
            update.Set.Add(new SetNode()
            {
                Field = new TableFieldNode() { Field = "MyField2" },
                Value = new ConstNode() { Value = 10 }
            });

            update.Where = GetWhere();

            var result = _visitor.Visit(update);

            Check(result);
        }

        [Fact]
        public void Delete()
        {

            var delete = new DeleteNode();

            delete.From = new Table() { Value = "MyTable1" };
            delete.Where = GetWhere();
            var result = _visitor.Visit(delete);

            Check(result);
        }

    }
}