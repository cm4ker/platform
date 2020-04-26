using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DDL.CreateTable
{
    public class DropTableQueryNode : SqlNode
    {
        private TableNode _table;

        public DropTableQueryNode(string tableName) : this(tableName, t => { })
        {
        }

        public DropTableQueryNode(string tableName, Action<TableNode> options)
        {
            _table = new TableNode(tableName);
            options(_table);

            Childs.AddRange(new Node[]
                {Tokens.DropToken, Tokens.SpaceToken, Tokens.TableToken, Tokens.SpaceToken, _table});
        }
    }
}