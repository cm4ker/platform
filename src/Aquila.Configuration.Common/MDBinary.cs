using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public class MDBinary : MDPrimitive, IEquatable<MDBinary>
    {
        public override Guid Guid => TypeConstants.Binary;

        public MDBinary()
        {
        }

        public MDBinary(int size)
        {
            Size = size;
        }

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
            return HashCode.Combine(Guid, Size);
        }
    }
}