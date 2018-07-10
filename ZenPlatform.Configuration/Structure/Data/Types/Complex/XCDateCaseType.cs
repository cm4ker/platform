using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public enum XCDateCaseType
    {
        [XmlEnum("DateTime")] DateTime,

        [XmlEnum("Date")] Date,

        [XmlEnum("Time")] Time,
    }
}