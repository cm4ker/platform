using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCGuid : XCPrimitiveType, IEquatable<XCGuid>
    {
        public override uint Id => 4;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }
        public bool Equals(XCGuid other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid
                && this.IsNullable == other.IsNullable;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCGuid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, IsNullable);
        }
    }
}