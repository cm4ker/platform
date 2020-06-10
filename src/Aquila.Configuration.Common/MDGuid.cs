using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public class MDGuid : MDPrimitive, IEquatable<MDGuid>
    {
        public override Guid Guid => TypeConstants.Guid;

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