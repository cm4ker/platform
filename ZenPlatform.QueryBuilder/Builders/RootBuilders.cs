using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Builders
{

    public class DDLQuery: IExpression
    {
        public static DDLQuery New()
        {
            return new DDLQuery();
        }

        public SSyntaxNode Expression => _expression;
        private Querys _expression = new Querys();

        public CreateBuilder Create()
        {
            var builder = new CreateBuilder();
            _expression.QueryList.Add(builder.Expression);
            return builder;
        }

        public AlterBuilder Alter()
        {
            var builder = new AlterBuilder();
            _expression.QueryList.Add(builder.Expression);
            return builder;
        }

        public DeleteBuilder Delete()
        {
            var builder = new DeleteBuilder();
            _expression.QueryList.Add(builder.Expression);
            return builder;
        }
        
        public CopyBuilder Copy()
        {
            var builder = new CopyBuilder();
            _expression.QueryList.Add(builder.Expression);
            return builder;
        }

        public RenameBuilder Rename()
        {
            var builder = new RenameBuilder();
            _expression.QueryList.Add(builder.Expression);
            return builder;
        }


    }

    public class CreateBuilder: IExpression
    {
        public SSyntaxNode Expression => _expression;
        private Querys _expression = new Querys();


        public CreateTableBuilder Table(string tableName)
        {

            CreateTable createTable = new CreateTable() { Table = new Table() { Value = tableName } };
            _expression.QueryList.Add(createTable);
            return new CreateTableBuilder(createTable);
        }

        public CreateColumnBuilder Column(string columnName)
        {

            var add = new AddColumn();

            _expression.QueryList.Add(add);
            return new CreateColumnBuilder(add).Column(columnName);

        }

        public CreateColumnBuilder Column(ColumnDefinition column)
        {
            var add = new AddColumn();

            _expression.QueryList.Add(add);
            return new CreateColumnBuilder(add).Column(column);
        }
    }

    public class AlterBuilder : IExpression
    {
        public SSyntaxNode Expression => _expression;

        private Querys _expression = new Querys();

        public AlterColumnBuilder Column(string columnName)
        {


            var alter = new AlterColumn();

            _expression.QueryList.Add(alter);
            return new AlterColumnBuilder(alter).Column(columnName);
        }

        public AlterColumnBuilder Column(ColumnDefinition column)
        {
            var alter = new AlterColumn();

            _expression.QueryList.Add(alter);
            return new AlterColumnBuilder(alter).Column(column);
        }

    }

    public class DeleteBuilder : IExpression
    {
        public SSyntaxNode Expression => _expression;

        private Querys _expression = new Querys();

        public DeleteTableBuilder Table(string tableName)
        {
            var dropTable = new DropTable() { Table = new Table() { Value = tableName } };
            _expression.QueryList.Add(dropTable);
            return new DeleteTableBuilder(dropTable);
        }

        public DeleteColumnBuilder Column(string columnName)
        {
            var dropColumn = new DropColumn()
            {
                Column = new Column() { Value = columnName }
            };

            _expression.QueryList.Add(dropColumn);
            return new DeleteColumnBuilder(dropColumn);
        }

    }

    public class CopyBuilder : IExpression
    {
        public SSyntaxNode Expression => _expression;

        private Querys _expression = new Querys();

        public CopyTableBuilder Table(string tableName)
        {
            var copy = new CopyTable();
            _expression.QueryList.Add(copy);
            return new CopyTableBuilder(copy).FromTable(tableName);
        }

        public CopyTableBuilder Table()
        {
            var copy = new CopyTable();
            _expression.QueryList.Add(copy);
            return new CopyTableBuilder(copy);
        }
    }


    public class RenameBuilder : IExpression
    {
        public SSyntaxNode Expression => _expression;

        private Querys _expression = new Querys();

        public RenameTableBuilder Table(string oldName = null)
        {
            var node = new RenameTableNode();
            if (!string.IsNullOrEmpty(oldName)) node.From = oldName;

            var builder = new RenameTableBuilder(node);
            _expression.QueryList.Add(node);
            return builder;
        }

    }

}