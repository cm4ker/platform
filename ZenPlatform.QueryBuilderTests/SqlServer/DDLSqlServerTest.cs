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
    public class DDLSqlServerTest
    {
        const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private DataContextManager _dataContextManager;
        private DataContext _context;
        private SQLQueryVisitor _visitor = new SQLQueryVisitor();
        public DDLSqlServerTest()
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
                Column = new Column() { Value = "Column1"},
                Type = new ColumnTypeInt(),
            };
            createTable.Columns.Add(column1);


            var column2 = new ColumnDefinition()
            {
                Column = new Column() { Value = "Column2" },
                Type = new ColumnTypeInt(),
            };
            column2.DefaultValue = 10;
            createTable.Columns.Add(column2);
            

            var fk = new ConstraintDefinitionForeignKey();
            fk.Columns.Add(column1.Column);
            fk.ForeignColumns.Add(column2.Column);
            fk.ForeignTable = new Table() { Value = "ForeignTable" };


            createTable.Constraints.Add(fk);


            



            var result = _visitor.Visit(createTable);

            Check(result);




            CreateTableBuilder builder = new CreateTableBuilder("MyTable");
            var nodes = builder
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

        
    }
}