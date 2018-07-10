using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
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

        public override DbType DBType
        {
            get { return DbType.Binary; }
        }

        public override Type CLRType => (IsNullable) ? typeof(byte?[]) : typeof(byte[]);
    }
}