using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntity : XCObjectTypeBase
    {
        private List<XCCommand> _predefinedCommands;

        public XCSingleEntity()
        {
            Properties = new ObservableCollection<IXCObjectProperty>();
            Properties.CollectionChanged += Properties_CollectionChanged;
            Modules = new XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule>(this);
            Commands = new List<XCCommand>();
            _predefinedCommands = new List<XCCommand>();

            InitPredefinedCommands();
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
        public ObservableCollection<IXCObjectProperty> Properties { get; }

        /// <summary>
        /// Коллекция модулей сущности
        /// </summary>
        public XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule> Modules { get; }

        /// <summary>
        /// Комманды, которые привязаны к сущности
        /// </summary>
        public List<XCCommand> Commands { get; }

        /// <inheritdoc />
        public override void LoadDependencies()
        {
            foreach (var property in Properties)
            {
                var configurationTypes = new List<XCTypeBase>();

                //После того, как мы получили все типы мы обязаны очистить битые ссылки и заменить их на нормальные
                foreach (var propertyType in property.GetUnprocessedPropertyTypes())
                {
                    if (propertyType is XCPrimitiveType)
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
                Root.Storage.GetId(property.Guid, ref id);
                property.Id = id;
            }
        }

        public override void Initialize()
        {
            if (Properties.FirstOrDefault(x => x.Unique) == null)
                Properties.Add(StandardEntityPropertyHelper.CreateUniqueProperty());

            if (Properties.FirstOrDefault(x => x.Name == "Name") == null)
                Properties.Add(StandardEntityPropertyHelper.CreateNameProperty());

            if (Properties.FirstOrDefault(x => x.Name == "Link") == null)
                Properties.Add(StandardEntityPropertyHelper.CreateLinkProperty(this));
        }

        public override IEnumerable<IXCObjectProperty> GetProperties()
        {
            return Properties;
        }

        /// <inheritdoc />
        public override IEnumerable<IXCProgramModule> GetProgramModules()
        {
            return Modules;
        }

        public override IXCObjectProperty CreateProperty()
        {
            var prop = new XCSingleEntityProperty();
            Properties.Add(prop);
            return prop;
        }

        public override IEnumerable<IXCCommand> GetCommands()
        {
            //Предопределенные комманды
            foreach (var command in _predefinedCommands)
            {
                yield return command;
            }

            foreach (var command in Commands)
            {
                yield return command;
            }
        }

        public override IXCCommand CreateCommand()
        {
            var cmd = new XCCommand(false);
            Commands.Add(cmd);
            return cmd;
        }


        /// <summary>
        /// Получить предопределённые комманды
        /// </summary>
        /// <returns></returns>
        private void InitPredefinedCommands()
        {
        }
    }

    public class XCSingleEntityLink : XCLinkTypeBase
    {
        public override string Name => $"{ParentType.Name}Link";

        public XCSingleEntityLink(IXCObjectType parentType)
        {
            ((IChildItem<IXCObjectType>) this).Parent = parentType;
        }
    }
}