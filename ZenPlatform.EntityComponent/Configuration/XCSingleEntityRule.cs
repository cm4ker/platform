using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.EntityComponent.Configuration
{

    /// <summary>
    /// Набор свойств к правилам для SingleEntityComponent
    /// </summary>
    [XmlRoot("SingleEntityRule")]
    public class XCSingleEntityRule : XCDataRuleBase
    {
        public XCSingleEntityRule()
        {
        }

        /// <summary>
        /// Позволять создавать элемент
        /// </summary>
        [XmlElement] public bool AllowCreate { get; set; }

        /// <summary>
        /// Позволять читать элемент
        /// </summary>
        [XmlElement] public bool AllowRead { get; set; }

        /// <summary>
        /// Позволять обновлять элемент
        /// </summary>
        [XmlElement] public bool AllowUpdate { get; set; }

        /// <summary>
        /// Позволять удалять элемент
        /// </summary>
        [XmlElement] public bool AllowDelete { get; set; }

        /// <summary>
        /// Позволять просматривать элемент
        /// </summary>
        [XmlElement] public bool AllowView { get; set; }

        /// <summary>
        /// Позволять интерактивно удалять элемент
        /// </summary>
        [XmlElement] public bool AllowInteractiveDelete { get; set; }

        /// <summary>
        /// Запрос РЛС на языке ZQL
        /// </summary>
        [XmlElement] public string RlsQuery { get; set; }
    }
}