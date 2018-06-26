using System;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive
{
    public class XCNumeric : XCPremitiveType
    {
        public override int Id => 5;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);


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