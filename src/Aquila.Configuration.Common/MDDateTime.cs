using System;
using System.Xml.Serialization;
using Aquila.Compiler.Contracts;

namespace Aquila.Configuration.Common
{
    public class MDDateTime : MDPrimitive, IEquatable<MDDateTime>
    {
        public override Guid Guid => TypeConstants.DateTime;

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