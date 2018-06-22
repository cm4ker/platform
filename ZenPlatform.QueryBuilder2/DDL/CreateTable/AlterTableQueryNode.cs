using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
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

        public void AlterColumn(string columnName, Func<TypeDefinitionFactory, TypeDefinitionNode> typeOption)
        {
            ColumnDefinitionNode col = new ColumnDefinitionNode(columnName);
            col.WithType(typeOption);
            Childs.AddRange(Tokens.AlterToken, Tokens.SpaceToken, Tokens.ColumnToken, Tokens.SpaceToken, col);
        }

        public void AddColumn(string columnName, Func<TypeDefinitionFactory, TypeDefinitionNode> typeOption)
        {
            ColumnDefinitionNode col = new ColumnDefinitionNode(columnName);
            col.WithType(typeOption);
            Childs.AddRange(Tokens.AddToken, Tokens.SpaceToken, col);
        }

        public void DropColumn(string columnName)
        {
            Childs.AddRange(Tokens.DropToken, Tokens.SpaceToken, Tokens.ColumnToken, Tokens.SpaceToken,
                new IdentifierNode(columnName));
        }
    }
}