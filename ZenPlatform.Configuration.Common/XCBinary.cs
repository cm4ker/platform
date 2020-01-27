using System;
using System.Data;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCBinary : MDType, IEquatable<XCBinary>
    {
        public override uint Id => 1;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1);


        public override string Name
        {
            get { return "Binary"; }
        }

        public int Size { get; set; }

        public bool Equals(XCBinary other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid && this.Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return Equals(obj as XCBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid,  Size);
        }
    }
}