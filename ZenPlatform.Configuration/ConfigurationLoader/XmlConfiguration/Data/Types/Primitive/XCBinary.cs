using System;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive
{
    public class XCBinary : XCPremitiveType
    {
        public override Guid Id
        {
            get { return new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 10); }
        }

        public override string Name
        {
            get { return "Binary"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DBType DBType
        {
            get { return DBType.Binary; }
        }

        public override Type CLRType
        {
            get { return typeof(byte[]); }
        }
    }
}