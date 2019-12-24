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

        private XCSingleEntityMetadata _metadata;
        private IXProperty _linkProperty;
        public XCSingleEntity(XCSingleEntityMetadata metadata)
        {


            _metadata = metadata;

            
        }


        public XCSingleEntityMetadata GetMetadata()
        {

            return _metadata;
        }

        public override bool HasProperties => true;



        /// <summary>
        /// Коллекция свойств сущности
        /// </summary>
        public IEnumerable<IXProperty> Properties => _metadata.Properties.Concat(_linkProperty != null ? new List<IXProperty>() { _linkProperty }: new List<IXProperty>());

        /// <summary>
        /// Коллекция модулей сущности
        /// </summary>
        public IEnumerable<XCSingleEntityModule> Modules => _metadata.Modules;

        public override string Name => _metadata.Name;

        public override Guid Guid => _metadata.EntityId;


        /// <summary>
        /// Комманды, которые привязаны к сущности
        /// </summary>
        public List<XCCommand> Commands => _metadata.Command;

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
                Root.Counter.GetId(property.Guid, ref id);
                property.Id = id;
            }
        }

        public override void Initialize()
        {
            if (Properties.FirstOrDefault(x => x.Unique) == null)
                _metadata.Properties.Add(StandardEntityPropertyHelper.CreateUniqueProperty());

            if (Properties.FirstOrDefault(x => x.Name == "Name") == null)
                _metadata.Properties.Add(StandardEntityPropertyHelper.CreateNameProperty());

            if (_linkProperty == null)
                _linkProperty = StandardEntityPropertyHelper.CreateLinkProperty(this);

        }

        public override IEnumerable<IXProperty> GetProperties()
        {
            return Properties;
        }

        /// <inheritdoc />
        public override IEnumerable<IXCProgramModule> GetProgramModules()
        {
            return Modules;
        }

        public override IEnumerable<IXCCommand> GetCommands()
        {

            foreach (var command in Commands)
            {
                yield return command;
            }
        }


        public override bool Equals(object obj)
        {
            return obj is XCSingleEntity entity &&
                   base.Equals(obj);
                   
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Properties, Modules, Commands);
        }
    }

    public class XCSingleEntityLink : XCLinkTypeBase
    {
        private readonly XCSingleEntityMetadata _metadata;
        public override string Name => $"{ParentType.Name}Link";

        public override Guid Guid => _metadata.LinkId;

        public override bool HasProperties => true;

        public XCSingleEntityLink(IXCObjectType parentType, XCSingleEntityMetadata metadata)
        {
            _metadata = metadata;
            ParentType = parentType;


        }

        public override IEnumerable<IXProperty> GetProperties()
        {
            return _metadata.Properties;
        }
    }
}