using System;

namespace ZenPlatform.Configuration.Common
{
    public class MDBinary : MDPrimitive, IEquatable<MDBinary>
    {
        public override uint Id => 1;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 1);


        public override string Name
        {
            get { return "Binary"; }
        }

        public int Size { get; set; }

        public bool Equals(MDBinary other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid && this.Size == other.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return Equals(obj as MDBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid,  Size);
        }
    }
}