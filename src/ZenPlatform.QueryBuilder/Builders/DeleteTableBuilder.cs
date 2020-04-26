using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class DeleteTableBuilder
    {
        private DropTable _dropTable;
        public DeleteTableBuilder(string tableName)
        {
            _dropTable = new DropTable() { Table = new Table() { Value = tableName } };
        }


        public DeleteTableBuilder(DropTable table)
        {
            _dropTable = table;
        }

        public DeleteTableBuilder IfExists()
        {
            _dropTable.IfExists = true;
            return this;
        }




    }
}
