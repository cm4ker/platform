using System;
using System.Data;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.Data
{
    public class PBoolean : PPrimetiveType
    {
        public override Guid Id
        {
            get
            {
                return new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);
            }
        }

        public override string Name
        {
            get { return "Boolean"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override DBType DBType
        {
            get { return DBType.Bit; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(bool?) : typeof(bool); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

    }
}