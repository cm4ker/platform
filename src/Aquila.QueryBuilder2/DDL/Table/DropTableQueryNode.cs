using System;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DDL.CreateTable
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