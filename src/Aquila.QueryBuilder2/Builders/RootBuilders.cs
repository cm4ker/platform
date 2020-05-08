using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{

    public class DDLQuery: IExpression
    {
        public static DDLQuery New()
        {
            return new DDLQuery();
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

        public RenameBuilder Rename()
        {
            var builder = new RenameBuilder();
            _expression.Nodes.Add(builder.Expression);
            return builder;
        }


    }


    public class Query : IExpression
    {
        public QuerySyntaxNode Expression => _expression;
        private Expression _expression = new Expression();

        public static Query New()
        {
            return new Query();
        }

        public SelectBuilder Select()
        {
            SelectNode selectNode = new SelectNode();
            _expression.Nodes.Add(selectNode);

            return new SelectBuilder(selectNode);


        }


        public InsertBuilder Insert()
        {
            InsertNode insertNode = new InsertNode();
            _expression.Nodes.Add(insertNode);

            return new InsertBuilder(insertNode);


        }

        public UpdateBuilder Update()
        {
            UpdateNode updateNode = new UpdateNode();
            _expression.Nodes.Add(updateNode);

            return new UpdateBuilder(updateNode);


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

            var add = new AddColumn();

            _expression.Nodes.Add(add);
            return new CreateColumnBuilder(add).Column(columnName);

        }

        public CreateColumnBuilder Column(ColumnDefinition column)
        {
            var add = new AddColumn();

            _expression.Nodes.Add(add);
            return new CreateColumnBuilder(add).Column(column);
        }
    }

    public class AlterBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public AlterColumnBuilder Column(string columnName)
        {


            var alter = new AlterColumn();

            _expression.Nodes.Add(alter);
            return new AlterColumnBuilder(alter).Column(columnName);
        }

        public AlterColumnBuilder Column(ColumnDefinition column)
        {
            var alter = new AlterColumn();

            _expression.Nodes.Add(alter);
            return new AlterColumnBuilder(alter).Column(column);
        }

    }

    public class DeleteBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public DeleteTableBuilder Table(string tableName)
        {
            var dropTable = new DropTable() { Table = new Table() { Value = tableName } };
            _expression.Nodes.Add(dropTable);
            return new DeleteTableBuilder(dropTable);
        }

        public DeleteColumnBuilder Column(string columnName)
        {
            var dropColumn = new DropColumn()
            {
                Column = new Column() { Value = columnName }
            };

            _expression.Nodes.Add(dropColumn);
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


    public class RenameBuilder : IExpression
    {
        public QuerySyntaxNode Expression => _expression;

        private Expression _expression = new Expression();

        public RenameTableBuilder Table(string oldName = null)
        {
            var node = new RenameTableNode();
            if (!string.IsNullOrEmpty(oldName)) node.From = oldName;

            var builder = new RenameTableBuilder(node);
            _expression.Nodes.Add(node);
            return builder;
        }

    }

}