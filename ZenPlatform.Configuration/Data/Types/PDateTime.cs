using System;
using System.Data;

namespace ZenPlatform.Configuration.Data
{
    public class PDateTime : PPrimetiveType
    {
        public override string Name
        {
            get { return "DateTime"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType DBType
        {
            get { return SqlDbType.DateTime; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(DateTime?) : typeof(DateTime); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }


    }
}