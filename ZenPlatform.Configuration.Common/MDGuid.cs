using System;
using System.Data;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class MDGuid : MDPrimitive, IEquatable<MDGuid>
    {
        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 4);

        public override string Name
        {
            get { return "Guid"; }
        }
        public bool Equals(MDGuid other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as MDGuid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }
    }
}