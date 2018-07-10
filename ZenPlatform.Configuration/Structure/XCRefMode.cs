using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    public enum XCRefMode
    {
        [XmlEnum("ToObject")]
        ToObject,

        [XmlEnum("ToComponent")]
        ToComponent,

        [XmlEnum("ToProperty")]
        ToProperty
    }
}