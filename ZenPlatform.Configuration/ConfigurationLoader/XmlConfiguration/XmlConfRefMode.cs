using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public enum XmlConfRefMode
    {
        [XmlEnum("ToObject")]
        ToObject,

        [XmlEnum("ToComponent")]
        ToComponent,

        [XmlEnum("ToProperty")]
        ToProperty
    }
}