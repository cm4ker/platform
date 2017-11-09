using System;
using System.Data;
using DbType = ZenPlatform.QueryBuilder.Schema.DBType;

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
        public override DbType DBType
        {
            get { return DbType.DateTime; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(DateTime?) : typeof(DateTime); }
        }
        public override int Precision { get; set; }
        public override int Scale { get; set; }


    }
}