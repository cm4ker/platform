using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public class MDInt : MDPrimitive, IEquatable<MDInt>
    {
        public override Guid Guid => TypeConstants.Int;

        public override string Name
        {
            get { return "Int"; }
        }

        public bool Equals(MDInt other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as MDInt);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }
    }
}