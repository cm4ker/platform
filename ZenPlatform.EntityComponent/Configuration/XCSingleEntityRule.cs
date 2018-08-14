using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.EntityComponent.Configuration
{
    [XmlRoot("SingleEntityRule")]
    public class XCSingleEntityRule : XCDataRuleBase
    {
        public XCSingleEntityRule()
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