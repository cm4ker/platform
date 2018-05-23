using System;
using System.Data;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.Data
{
    public class PString : PPrimetiveType
    {
        public override Guid Id
        {
            get
            {
                return new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);
            }
        }

        public override string Name
        {
            get { return "String"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DBType DBType
        {
            get { return DBType.NVarChar; }
        }

        public override Type CLRType
        {
            get { return typeof(string); }
        }

    }
}