using System;
using System.Data;

namespace SqlPlusDbSync.Configuration
{
    public class PNumeric : PPrimetiveType
    {
        public override string Name
        {
            get { return "Numeric"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType DBType
        {
            get { return SqlDbType.Decimal; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(decimal?) : typeof(decimal); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }


    }
}