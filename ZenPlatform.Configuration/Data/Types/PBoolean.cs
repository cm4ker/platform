using System;
using System.Data;

namespace ZenPlatform.Configuration.Data
{
    public class PBoolean : PPrimetiveType
    {
        public override string Name
        {
            get { return "Boolean"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType DBType
        {
            get { return SqlDbType.Bit; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(bool?) : typeof(bool); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

    }
}