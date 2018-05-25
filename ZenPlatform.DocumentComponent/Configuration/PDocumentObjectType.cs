using System;
using System.Xml;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    public class DocumentProperty : XCObjectPropertyBase, IChildItem<Document>
    {
        private Document _parent;

        public DocumentProperty() : base()
        {
        }

        [XmlIgnore] public Document Parent => _parent;

        Document IChildItem<Document>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    public class Document : XCObjectTypeBase
    {
        public Document()
        {
            Properties = new ChildItemCollection<Document, DocumentProperty>(this);
        }

        public ChildItemCollection<Document, DocumentProperty> Properties { get; }

        /// <summary>
        /// Имя связанной таблицы
        /// 
        /// При миграции присваивается движком. В последствии хранится в конфигурации.
        /// </summary>
        [XmlElement]
        public string RelTableName { get; set; }
    }
}