using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class CopyTableBuilder
    {

        private CopyTable _copyTable;

        public CopyTableBuilder(CopyTable copyTable)
        {
            _copyTable = copyTable;
        }


        public CopyTableBuilder FromTable(string sourceTable)
        {
            _copyTable.Table = new Table() { Value = sourceTable };

            return this;
        }

        public CopyTableBuilder ToTable(string destinationTable)
        {
            _copyTable.DstTable = new Table() { Value = destinationTable };

            return this;
        }



    }
}
