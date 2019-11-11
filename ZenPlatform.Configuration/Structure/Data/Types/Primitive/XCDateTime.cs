using System;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCDateTime : XCPrimitiveType, IEquatable<XCDateTime>
    {
        public override uint Id => 3;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3);

        public override string Name
        {
            get { return "DateTime"; }
        }

        public bool Equals(XCDateTime other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid
                && this.IsNullable == other.IsNullable;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, IsNullable);
        }



    }
}