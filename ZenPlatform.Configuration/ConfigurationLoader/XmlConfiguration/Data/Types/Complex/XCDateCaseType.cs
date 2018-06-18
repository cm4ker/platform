using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public enum XCDateCaseType
    {
        [XmlEnum("DateTime")] DateTime,

        [XmlEnum("Date")] Date,

        [XmlEnum("Time")] Time,
    }
}