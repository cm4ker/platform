using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCGuid : XCPrimitiveType
    {
        public override uint Id => 4;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }

        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override DbType DBType
        {
            get { return DbType.Guid; }
        }

        public override Type CLRType
        {
            get { return (IsNullable) ? typeof(Guid?) : typeof(Guid); }
        }
    }
}