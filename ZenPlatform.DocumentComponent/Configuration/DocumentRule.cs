using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    [XmlRoot("DocumentRule")]
    public class DocumentRule : XCDataRuleBase
    {
        public DocumentRule()
        {
        }

        [XmlElement] public bool AllowCreate { get; set; }

        [XmlElement] public bool AllowRead { get; set; }

        [XmlElement] public bool AllowUpdate { get; set; }

        [XmlElement] public bool AllowDelete { get; set; }

        [XmlElement] public bool AllowView { get; set; }

        [XmlElement] public bool AllowInteractiveDelete { get; set; }
    }
}