﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    [XmlRoot("SingleEntity")]
    public class XCSingleEntity : XCObjectTypeBase
    {
        public XCSingleEntity()
        {
            Properties = new ChildItemCollection<XCSingleEntity, XCSingleEntityProperty>(this);
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


        /// <summary>
        /// Коллекция свойств сущности
        /// </summary>
        [XmlArray]
        [XmlArrayItem(ElementName = "Property", Type = typeof(XCSingleEntityProperty))]
        public ChildItemCollection<XCSingleEntity, XCSingleEntityProperty> Properties { get; }

        /// <summary>
        /// Имя связанной таблицы документа
        /// 
        /// При миграции присваивается движком. В последствии хранится в структурых инициализации конкретной базы.
        /// </summary>
        //TODO: Продумать структуру, в которой будут храниться сопоставление Тип -> Дополнительные настройки компонента 
        /*
         * Результаты раздумий: Все мапинги должны быть в БД. 
         */
        [XmlIgnore]
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

        public override void Initialize()
        {
            Parent.ComponentImpl.Caches.Add(this.Name, new MemoryCache(this.Name));

            if (Properties.FirstOrDefault(x => x.Unique) == null)
                Properties.Add(StandartDocumentPropertyHelper.CreateUniqueProperty());
        }
    }
}