using System.Collections.Generic;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Параметр сессии
    /// </summary>
    public class XCSessionSetting : IXCSessionSetting
    {
        private IXCRoot _parent;

        public IXCRoot Parent => _parent;

        /// <summary>
        /// Имя параметра сессии
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Тип параметра сессии
        /// </summary>
        [XmlArray]
        [XmlArrayItem(ElementName = "Type", Type = typeof(UnknownType))]
        public List<IXCType> Types { get; }

        IXCRoot IChildItem<IXCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}