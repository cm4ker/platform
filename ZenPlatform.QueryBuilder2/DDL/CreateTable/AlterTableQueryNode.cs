using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.From;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public class AlterTableQueryNode : SqlNode
    {
        private TableNode _table;

        public AlterTableQueryNode(string tableName, Action<TableNode> options)
        {
            _table = new TableNode(tableName);
            options(_table);

            Childs.AddRange(new SqlNode[]
                {Tokens.AlterToken, Tokens.SpaceToken, Tokens.TableToken, Tokens.SpaceToken, _table});
        }

        public void AlterColumn()
        {
        }

        public void Add()
        {
        }

        public void Drop()
        {
        }
    }
}