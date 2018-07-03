using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Tokens;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
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

            Childs.AddRange(new SqlNode[]
                {Tokens.DropToken, Tokens.SpaceToken, Tokens.TableToken, Tokens.SpaceToken, _table});
        }
    }
}