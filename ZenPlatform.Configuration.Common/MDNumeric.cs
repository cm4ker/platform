using System;

namespace ZenPlatform.Configuration.Common
{
    public class MDNumeric : MDPrimitive, IEquatable<MDNumeric>
    {
        public override uint Id => 5;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 5);


        public override string Name
        {
            get { return "Numeric"; }
        }

        public int Precision { get; set; }
        public int Scale { get; set; }


        public bool Equals(MDNumeric other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid  &&
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