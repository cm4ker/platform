using System;
using System.Text;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.Columns;
using Aquila.QueryBuilder.Common.Factoryes;
using Aquila.QueryBuilder.Common.Operations;
using Aquila.QueryBuilder.Common.SqlTokens;

namespace Aquila.QueryBuilder.DDL.Table
{
    public class AlterTableQueryNode : SqlNode
    {
        private TableNode _table;

        public AlterTableQueryNode(string tableName, Action<TableNode> options = null)
        {
            _table = new TableNode(tableName);
            options?.Invoke(_table);

            Childs.AddRange(Tokens.AlterToken, Tokens.SpaceToken, Tokens.TableToken, Tokens.SpaceToken, _table,
                Tokens.SpaceToken);
        }

        public AlterTableQueryNode AlterColumn(string columnName,
            Func<TypeDefinitionFactory, TypeDefinitionNode> typeOption)
        {
            ColumnDefinitionNode col = new ColumnDefinitionNode(columnName);
            col.WithType(typeOption);
            Childs.AddRange(Tokens.AlterToken, Tokens.SpaceToken, Tokens.ColumnToken, Tokens.SpaceToken, col);
            return this;
        }

        public AlterTableQueryNode AddColumn(string columnName,
            Func<TypeDefinitionFactory, TypeDefinitionNode> typeOption)
        {
            ColumnDefinitionNode col = new ColumnDefinitionNode(columnName);
            col.WithType(typeOption);
            Childs.AddRange(Tokens.AddToken, Tokens.SpaceToken, col);
            return this;
        }

        public AlterTableQueryNode DropColumn(string columnName)
        {
            Childs.AddRange(Tokens.DropToken, Tokens.SpaceToken, Tokens.ColumnToken, Tokens.SpaceToken,
                new IdentifierNode(columnName));

            return this;
        }
    }

    public class RenameTableQueryNode : SqlNode
    {
        private readonly string _newTableName;
        private TableNode _table;

        public RenameTableQueryNode(string tableName, string newTableName, Action<TableNode> options = null)
        {
            _newTableName = newTableName;
            _table = new TableNode(tableName);
            options?.Invoke(_table);
        }

        public TableNode Table => _table;

        public IdentifierNode NewTableName => new IdentifierNode(_newTableName);
    }
}