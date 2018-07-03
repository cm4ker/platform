using System;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Primitive
{
    public class XCBinary : XCPremitiveType
    {
        public override int Id => 1;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1);


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

        public override Type CLRType => (IsNullable) ? typeof(byte?[]) : typeof(byte[]);
    }
}