using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.QueryBuilder.Tests
{
    public class BuildersTest
    {

        private SQLVisitorBase _visitor;
        public BuildersTest()
        {
            _visitor = new SQLVisitorBase();
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

            //Check(result);




            var query = DDLQuery.New();
                query
                .Create().Table("MyTable")
                .WithColumn("Column1").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("Column2").AsInt32().WithDefaultValue(10).Unique().ForeignKey("ForeignTable", "ForeignColumn")
                ;


            var result2 = _visitor.Visit(query.Expression);

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


            var query = DDLQuery.New();
            query.Alter().Column("Column1").AsInt32().NotNullable().OnTable("MyTable");


            var result2 = _visitor.Visit(query.Expression);

            Assert.Equal(result2, result);

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

            var query = DDLQuery.New();
            query.Delete().Column("Column1").OnTable("MyTable");


            var result2 = _visitor.Visit(query.Expression);

            Assert.Equal(result2, result);
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


        }

        [Fact]
        public void AddForeignKeyConstraintWithName()
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


        }

    }
}
