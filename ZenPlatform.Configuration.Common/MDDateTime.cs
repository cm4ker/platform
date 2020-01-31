using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.Configuration.Structure.Data.Types.Primitive
{
    public class MDDateTime : MDPrimitive, IEquatable<MDDateTime>
    {
        public override uint Id => 3;

        public override Guid Guid => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 3);

        public override string Name
        {
            get { return "DateTime"; }
        }

        public bool Equals(MDDateTime other)
        {
            if (other == null) return false;

            return this.Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as MDDateTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid);
        }
    }
    
    public enum XCDateCaseType
    {
        [XmlEnum("DateTime")] DateTime,

        [XmlEnum("Date")] Date,

        [XmlEnum("Time")] Time,
    }
}