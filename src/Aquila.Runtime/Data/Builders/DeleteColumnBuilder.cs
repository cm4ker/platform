using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class DeleteColumnBuilder
    {
        private DropColumn _dropColumn;

        public DeleteColumnBuilder(string columnName)
        {
            _dropColumn = new DropColumn()
            {
                Column = new Column() { Value = columnName }
            };
        }


        public DeleteColumnBuilder(DropColumn dropColumn)
        {
            _dropColumn = dropColumn;
        }

        public DeleteColumnBuilder OnTable(string tableName)
        {
            _dropColumn.Table = new Table() { Value = tableName };
            return this;
        }
    }
}