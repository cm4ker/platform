using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Параметр сессии
    /// </summary>
    public class XCSessionSetting : IChildItem<XCRoot>
    {
        private XCRoot _parent;

        public XCRoot Parent => _parent;

        /// <summary>
        /// Имя параметра сессии
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Тип параметра сессии
        /// </summary>
        [XmlArray]
        [XmlArrayItem(ElementName = "Type", Type = typeof(XCUnknownType))]
        public List<XCTypeBase> Types { get; }

        XCRoot IChildItem<XCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}