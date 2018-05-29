using System;
using System.Xml;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    /// <summary>
    /// Свойство документа
    /// </summary>
    public class DocumentProperty : XCObjectPropertyBase, IChildItem<Document>
    {
        private Document _parent;

        public DocumentProperty()
        {
        }

        [XmlIgnore] public Document Parent => _parent;

        Document IChildItem<Document>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}