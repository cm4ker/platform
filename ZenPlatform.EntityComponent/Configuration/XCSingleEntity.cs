using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    [XmlRoot("SingleEntity")]
    public class XCSingleEntity : XCObjectTypeBase
    {
        public XCSingleEntity()
        {
            Properties = new XCPropertyCollection<XCSingleEntity, XCSingleEntityProperty>(this);
            Properties.CollectionChanged += Properties_CollectionChanged;

            Modules = new XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule>(this);
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
                            if (mailElement.Name == compareElement.Name
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
        public XCPropertyCollection<XCSingleEntity, XCSingleEntityProperty> Properties { get; }

        /// <summary>
        /// Коллекция модулей сущности
        /// </summary>
        [XmlArray]
        [XmlArrayItem(ElementName = "Modules", Type = typeof(XCSingleEntityModule))]
        public XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule> Modules { get; }

        /// <summary>
        /// Имя связанной таблицы документа
        /// 
        /// При миграции присваивается движком. В последствии хранится в структурых инициализации конкретной базы.
        /// </summary>
        //TODO: Продумать структуру, в которой будут храниться сопоставление Тип -> Дополнительные настройки компонента 
        /*
         * Результаты раздумий: Все мапинги должны быть в БД, а не в конфигурации. Оставляю TODO
         * выше просто для того, чтобы можно было поразмышлять,  вдруг я был не прав
         */
        [XmlIgnore]
        public string RelTableName { get; set; }

        /// <inheritdoc />
        public override void LoadDependencies()
        {
            foreach (var property in Properties)
            {
                var configurationTypes = new List<XCTypeBase>();

                //После того, как мы получили все типы мы обязаны очистить битые ссылки и заменить их на нормальные
                foreach (var propertyType in property.GetUnprocessedPropertyTypes())
                {
                    if (propertyType is XCPremitiveType)
                        property.Types.Add(propertyType);
                    if (propertyType is XCUnknownType)
                    {
                        var type = Data.PlatformTypes.FirstOrDefault(x => x.Guid == propertyType.Guid);
                        //Если по какой то причине тип поля не найден, в таком случае считаем, что конфигурация битая и выкидываем исключение
                        if (type == null) throw new Exception("Invalid configuration");

                        property.Types.Add(type);
                    }
                }

                var id = property.Id;
                property.Parent.Root.Storage.GetId(property.Guid, ref id);
                property.Id = id;
            }
        }

        public override void Initialize()
        {
            if (Properties.FirstOrDefault(x => x.Unique) == null)
                Properties.Add(StandardEntityPropertyHelper.CreateUniqueProperty());
        }

        public override IEnumerable<XCObjectPropertyBase> GetProperties()
        {
            return Properties;
        }

        /// <inheritdoc />
        public override IEnumerable<XCProgramModuleBase> GetProgramModules()
        {
            return Modules;
        }
    }
}