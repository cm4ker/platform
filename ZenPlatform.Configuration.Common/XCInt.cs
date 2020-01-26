using System;


namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class XCInt : MDType, IEquatable<XCInt>
    {
        public override uint Id => 7;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 7);

        public override string Name
        {
            get { return "Int"; }
        }

        public bool Equals(XCInt other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCInt);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }
    }
}