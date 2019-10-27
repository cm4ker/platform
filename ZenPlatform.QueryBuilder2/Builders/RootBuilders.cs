using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{

    public class Query: IExpression
    {
        public static Query New()
        {
            return new Query();
        }


        public QuerySyntaxNode Expression => _expression;
        private Expression _expression = new Expression();
        public CreateBuilder Create()
        {
            var builder = new CreateBuilder();
            _expression.Nodes.Add(builder.Expression);
            return builder;
        }


        public AlterBuilder Alter()
        {
            var builder = new AlterBuilder();
            _expression.Nodes.Add(builder.Expression);
            return builder;
        }

        public DeleteBuilder Delete()
        {
            var builder = new DeleteBuilder();
            _expression.Nodes.Add(builder.Expression);
            return builder;
        }
        
        public CopyBuilder Copy()
        {
            var builder = new CopyBuilder();
            _expression.Nodes.Add(builder.Expression);
            return builder;
        }
    }
    public class CreateBuilder: IExpression
    {
        public QuerySyntaxNode Expression => _expression;
        private Expression _expression = new Expression();


        public CreateTableBuilder Table(string tableName)
        {

            CreateTable createTable = new CreateTable() { Table = new Table() { Value = tableName } };
            _expression.Nodes.Add(createTable);
            return new CreateTableBuilder(createTable);
        }

        public CreateColumnBuilder Column(string columnName)
        {
            var column = new ColumnDefinition()
            {
                Column = new Column() { Value = columnName }
            };
            return new CreateColumnBuilder(column);
        }
    }

    public class AlterBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public AlterColumnBuilder Column(string columName)
        {
            return new AlterColumnBuilder(_expression).Column(columName);
        }

        public AlterColumnBuilder Column(ColumnDefinition column)
        {
            return new AlterColumnBuilder(_expression).Column(column);
        }

    }

    public class DeleteBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public DeleteTableBuilder Table(string tableName)
        {
            var _dropTable = new DropTable() { Table = new Table() { Value = tableName } };
            return new DeleteTableBuilder(_dropTable);
        }

        public DeleteColumnBuilder Column(string columnName)
        {
            var dropColumn = new DropColumn()
            {
                Column = new Column() { Value = columnName }
            };
            return new DeleteColumnBuilder(dropColumn);
        }

    }

    public class CopyBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public CopyTableBuilder Table(string tableName)
        {
            var copy = new CopyTable();
            _expression.Nodes.Add(copy);
            return new CopyTableBuilder(copy).FromTable(tableName);
        }

        public CopyTableBuilder Table()
        {
            var copy = new CopyTable();
            _expression.Nodes.Add(copy);
            return new CopyTableBuilder(copy);
        }
    }

}