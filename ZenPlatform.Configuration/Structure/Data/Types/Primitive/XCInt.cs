using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCInt : XCPremitiveType
    {
        public override uint Id => 7;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);

        public override string Name
        {
            get { return "Int"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DbType DBType
        {
            get { return DbType.Int32; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(int?) : typeof(int); }
        }
    }
}