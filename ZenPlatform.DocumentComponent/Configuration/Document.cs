using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.DocumentComponent.Configuration
{
    [XmlRoot("Document")]
    public class Document : XCObjectTypeBase
    {
        public Document()
        {
            Properties = new ChildItemCollection<Document, DocumentProperty>(this);
        }


        [XmlElement] public bool UseNumaration { get; set; }

        [XmlElement] public string NumericPattern { get; set; }

        [XmlElement] public bool CanBePosted { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(DocumentProperty))]
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