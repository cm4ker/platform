using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
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
