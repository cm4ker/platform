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

        public DDLQuery Add(Action<QueryMachine> action)
        {
            QueryMachine machine = new QueryMachine();

            action(machine);

            return Add((SSyntaxNode)machine.pop());
        }

        public DDLQuery Add(SSyntaxNode syntax)
        {
            _expression.QueryList.Add(syntax);

            return this;
        }

        public CreateBuilder Create()
        {
            var builder = new CreateBuilder(_expression);
            return builder;
        }

        public AlterBuilder Alter()
        {
            var builder = new AlterBuilder(_expression);
            return builder;
        }

        public DeleteBuilder Delete()
        {
            var builder = new DeleteBuilder(_expression);
            return builder;
        }
        
        public CopyBuilder Copy()
        {
            var builder = new CopyBuilder(_expression);
            return builder;
        }

        public RenameBuilder Rename()
        {
            var builder = new RenameBuilder(_expression);
            return builder;
        }


    }

    public class CreateBuilder
    {
        private Querys _querys ;

        public CreateBuilder(Querys querys)
        {
            _querys = querys;
        }


        public CreateTableBuilder Table(string tableName)
        {

            CreateTable createTable = new CreateTable() { Table = new Table() { Value = tableName } };
            _querys.QueryList.Add(createTable);
            return new CreateTableBuilder(createTable);
        }

        public CreateColumnBuilder Column(string columnName)
        {

            var add = new AddColumn();

            _querys.QueryList.Add(add);
            return new CreateColumnBuilder(add).Column(columnName);

        }

        public CreateColumnBuilder Column(ColumnDefinition column)
        {
            var add = new AddColumn();

            _querys.QueryList.Add(add);
            return new CreateColumnBuilder(add).Column(column);
        }
    }

    public class AlterBuilder 
    {
        private Querys _querys;

        public AlterBuilder(Querys querys)
        {
            _querys = querys;
        }

        public AlterColumnBuilder Column(string columnName)
        {


            var alter = new AlterColumn();

            _querys.QueryList.Add(alter);
            return new AlterColumnBuilder(alter).Column(columnName);
        }

        public AlterColumnBuilder Column(ColumnDefinition column)
        {
            var alter = new AlterColumn();

            _querys.QueryList.Add(alter);
            return new AlterColumnBuilder(alter).Column(column);
        }

    }

    public class DeleteBuilder 
    {
        private Querys _querys;

        public DeleteBuilder(Querys querys)
        {
            _querys = querys;
        }

        public DeleteTableBuilder Table(string tableName)
        {
            var dropTable = new DropTable() { Table = new Table() { Value = tableName } };
            _querys.QueryList.Add(dropTable);
            return new DeleteTableBuilder(dropTable);
        }

        public DeleteColumnBuilder Column(string columnName)
        {
            var dropColumn = new DropColumn()
            {
                Column = new Column() { Value = columnName }
            };

            _querys.QueryList.Add(dropColumn);
            return new DeleteColumnBuilder(dropColumn);
        }

    }

    public class CopyBuilder 
    {
        private Querys _querys;

        public CopyBuilder(Querys querys)
        {
            _querys = querys;
        }

        public CopyTableBuilder Table(string tableName)
        {
            var copy = new CopyTable();
            _querys.QueryList.Add(copy);
            return new CopyTableBuilder(copy).FromTable(tableName);
        }

        public CopyTableBuilder Table()
        {
            var copy = new CopyTable();
            _querys.QueryList.Add(copy);
            return new CopyTableBuilder(copy);
        }
    }


    public class RenameBuilder 
    {
        private Querys _querys;

        public RenameBuilder(Querys querys)
        {
            _querys = querys;
        }

        public RenameTableBuilder Table(string oldName = null)
        {
            var node = new RenameTableNode();
            if (!string.IsNullOrEmpty(oldName)) node.From = new Table() { Value = oldName };

            var builder = new RenameTableBuilder(node);
            _querys.QueryList.Add(node);
            return builder;
        }

    }

}