using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCNumeric : XCPrimitiveType, IEquatable<XCNumeric>
    {
        public override uint Id => 5;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);


        public override string Name
        {
            get { return "Numeric"; }
        }

        public int Precision { get; set; }
        public int Scale { get; set; }


        public bool Equals(XCNumeric other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid && this.IsNullable == other.IsNullable &&
                this.Scale == other.Scale && this.Precision == other.Precision;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, IsNullable, Scale, Precision);
        }
    }
}