using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
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

        private void Properties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public override void LoadDependencies()
        {
            foreach (var property in Properties)
            {
                var configurationTypes = new List<XCTypeBase>();

                foreach (var propertyType in property.Types)
                {
                    var type = Data.PlatformTypes.First(x => x.Guid == propertyType.Guid && x.Id == propertyType.Id);
                    //Если по какой то причине тип поля не найден, в таком случае считаем, что конфигурация битая и выкидываем исключение
                    if (type == null) throw new Exception("Invalid configuration");

                    configurationTypes.Add(type);
                }

                //После того, как мы получили все типы мы обязаны очистить битые ссылки и заменить их на нормальные 
                property.Types.Clear();
                property.Types.AddRange(configurationTypes);
            }
        }
    }
}