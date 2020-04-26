using System;

namespace ZenPlatform.Configuration.Common
{
    public class MDBoolean : MDPrimitive, IEquatable<MDBoolean>
    {
        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 2);

        public override string Name
        {
            get { return "Boolean"; }
        }

        public bool Equals(MDBoolean other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false; 
            return Equals(obj as MDBoolean);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }


    }
}