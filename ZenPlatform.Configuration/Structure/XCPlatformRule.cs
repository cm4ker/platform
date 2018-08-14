using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Правла для роли на уровне платформы
    /// Эти правила общедоступны
    /// </summary>
    public class XCPlatformRule : IChildItem<XCRole>
    {
        private XCRole _parent;

        [XmlElement] public bool IsDataAdministrator { get; set; }

        [XmlElement] public bool CanUpdateDatabase { get; set; }

        [XmlElement] public bool CanMonopolisticMode { get; set; }

        [XmlElement] public bool IsActiveUser { get; set; }

        [XmlElement] public bool CanActionLogView { get; set; }

        [XmlElement] public bool CanUseThinClient { get; set; }

        [XmlElement] public bool CanUseWebClient { get; set; }

        [XmlElement] public bool CanUseExternalConnection { get; set; }

        [XmlElement] public bool CanOpenExternalModules { get; set; }

        [XmlIgnore] public XCRole Parent => _parent;

        XCRole IChildItem<XCRole>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}