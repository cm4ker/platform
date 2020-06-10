using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public sealed class MDNumeric : MDPrimitive, IEquatable<MDNumeric>
    {
        public MDNumeric()
        {
        }

        public MDNumeric(int scale, int precision)
        {
            Scale = scale;
            Precision = precision;
        }

        public override Guid Guid => TypeConstants.Numeric;


        public override string Name
        {
            get { return "Numeric"; }
        }

        public int Precision { get; set; }
        public int Scale { get; set; }


        public bool Equals(MDNumeric other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid &&
                   this.Scale == other.Scale && this.Precision == other.Precision;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as MDBinary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Scale, Precision);
        }
    }
}