using System;
using System.Data;

namespace SqlPlusDbSync.Configuration
{
    public class PGuid : PPrimetiveType
    {
        public override string Name
        {
            get { return "Guid"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override SqlDbType DBType
        {
            get { return SqlDbType.UniqueIdentifier; }
        }

        public override Type CLRType
        {
            get { return typeof(Guid); }
        }

    }
}