using System;
using System.Data;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.Data
{
    public class PNumeric : PPrimetiveType
    {
        public override Guid Id
        {
            get
            {
                return new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);
            }
        }

        public override string Name
        {
            get { return "Numeric"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override DBType DBType
        {
            get { return DBType.Decimal; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(decimal?) : typeof(decimal); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }


    }
}