using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public class MDBoolean : MDPrimitive, IEquatable<MDBoolean>
    {
        public override Guid Guid => TypeConstants.Boolean;

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