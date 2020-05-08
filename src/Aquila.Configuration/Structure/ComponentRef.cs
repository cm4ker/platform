using System;
using Aquila.Configuration.Contracts;

namespace Aquila.Configuration.Structure
{
    public class ComponentRef : IComponentRef
    {
        public string Name { get; set; }
        public string Entry { get; set; }

        protected bool Equals(ComponentRef other)
        {
            return Name == other.Name && Entry == other.Entry;
        }

        public bool Equals(IComponentRef other)
        {
            if (other is ComponentRef obj)
                return Name == obj.Name && Entry == obj.Entry;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Entry);
        }
    }
}