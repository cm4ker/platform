using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.Common.Operations;
using ZenPlatform.QueryBuilder2.Common.Table;
using ZenPlatform.QueryBuilder2.Common.Tokens;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public class CreateTableQueryNode : SqlNode
    {
        private TableNode _table;
        private TableDefinitionNode _definition;

        public CreateTableQueryNode()
        {
            Childs.Add(Tokens.CreateToken);
            Childs.Add(Tokens.SpaceToken);
            Childs.Add(Tokens.TableToken);
            Childs.Add(Tokens.SpaceToken);

            _definition = new TableDefinitionNode();
        }

        public CreateTableQueryNode(string tableName, Action<TableNode> options = null) : this()
        {
            _table = new TableNode(tableName);
            options?.Invoke(_table);

            Childs.Add(_table);
            Childs.Add(Tokens.LeftBracketToken);
            Childs.Add(_definition);
            Childs.Add(Tokens.RightBracketToken);
        }

        public CreateTableQueryNode(string schema, string tableName) : this(tableName, t => t.WithSchema(schema))
        {

        }

        public CreateTableQueryNode WithColumn(string columnName)
        {
            _definition.WithColumn(columnName);
            return this;
        }

        public CreateTableQueryNode WithColumn(string columnName, Func<TypeDefinitionFactory, TypeDefinitionNode> option)
        {
            _definition.WithColumn(columnName, option);
            return this;
        }
    }
}