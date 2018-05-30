using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.DocumentComponent.Configuration
{
    [XmlRoot("Document")]
    public class Document : XCObjectTypeBase
    {
        private bool _canBePosted;

        public Document()
        {
            Properties = new ChildItemCollection<Document, DocumentProperty>(this);
            Properties.CollectionChanged += Properties_CollectionChanged;
        }

        private void Properties_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < Properties.Count - 1; i++)
                {
                    for (int j = i + 1; j < Properties.Count; j++)
                    {
                        var mailElement = Properties[i];
                        var compareElement = Properties[j];

                        try
                        {
                            if (mailElement.Alias == compareElement.Alias
                                || (!string.IsNullOrEmpty(mailElement.DatabaseColumnName)
                                    && !string.IsNullOrEmpty(compareElement.DatabaseColumnName)
                                    && mailElement.DatabaseColumnName == compareElement.DatabaseColumnName))
                            {
                                throw new Exception("Свойства не целостны");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Write("Error");
                        }
                    }
                }
            }
        }

        [XmlElement] public bool UseNumaration { get; set; }

        [XmlElement] public string NumericPattern { get; set; }

        /// <summary>
        /// Имеет ли документ силу или нет
        /// </summary>
        [XmlElement]
        public bool CanBePosted
        {
            get => _canBePosted;
            set
            {
                _canBePosted = value;

                var prop = Properties.FirstOrDefault(x => x.Alias == "Posted");
                if (value && prop is null)
                {
                    Properties.Add(StandartDocumentPropertyHelper.CreatePostedProperty());
                }

                if (!value)
                {
                    Properties.Remove(prop);
                }
            }
        }

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