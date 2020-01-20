using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntity : XCObjectTypeBase
    {
        private MDSingleEntity _metadata;
        private IXCProperty _linkProperty;
        private bool _isInitialized;

        public XCSingleEntity(MDSingleEntity metadata)
        {
            _metadata = metadata;
        }

        public MDSingleEntity GetMetadata()
        {
            return _metadata;
        }

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool HasCommands => true;

        public override bool HasProperties => true;

        public override bool HasModules => true;

        public override bool HasDatabaseUsed => true;

        /// <summary>
        /// Коллекция свойств сущности
        /// </summary>
        public IEnumerable<IXCProperty> Properties => GetProperties();

        /// <summary>
        /// Коллекция таблиц сущности
        /// </summary>
        public IEnumerable<IXCTable> Tables => GetTables();

        /// <summary>
        /// Коллекция модулей сущности
        /// </summary>
        public IEnumerable<XCSingleEntityModule> Modules => _metadata.Modules;

        public override string Name => _metadata.Name;

        public override string RelTableName
        {
            get => _metadata.TableName;
            set { _metadata.TableName = value; }
        }

        public override Guid Guid => _metadata.EntityId;

        /// <summary>
        /// Комманды, которые привязаны к сущности
        /// </summary>
        public List<XCCommand> Commands => _metadata.Command;

        private void LoadDependenciesProperties(IEnumerable<IXCProperty> properties)
        {
            foreach (var property in properties)
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

        /// <inheritdoc />
        public override void LoadDependencies()
        {
            LoadDependenciesProperties(GetProperties());

            foreach (var table in Tables)
            {
                var id = table.Id;
                Root.Counter.GetId(table.Guid, ref id);
                table.Id = id;

                LoadDependenciesProperties(table.GetProperties());
            }
        }

        public override void Initialize()
        {
            if (_isInitialized) return;

            if (Properties.FirstOrDefault(x => x.Unique) == null)
                _metadata.Properties.Add(StandardEntityPropertyHelper.CreateUniqueProperty());

            if (Properties.FirstOrDefault(x => x.Name == "Name") == null)
                _metadata.Properties.Add(StandardEntityPropertyHelper.CreateNameProperty());

            if (_linkProperty == null)
                _linkProperty = StandardEntityPropertyHelper.CreateLinkProperty(this);

            _isInitialized = true;
        }

        public override IEnumerable<IXCProperty> GetProperties()
        {
            foreach (var property in _metadata.Properties)
            {
                yield return property;
            }

            if (_linkProperty != null)
                yield return _linkProperty;
        }

        public override IEnumerable<IXCTable> GetTables()
        {
            foreach (var table in _metadata.Tables)
            {
                yield return new XCSingleEntityTable(table);
            }
        }

        public override IEnumerable<IXCProgramModule> GetProgramModules()
        {
            return Modules;
        }

        public override IEnumerable<IXCCommand> GetCommands()
        {
            return Commands;
        }

        public override bool Equals(object obj)
        {
            return obj is XCSingleEntity entity &&
                   Guid.Equals(entity.Guid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Guid);
        }
    }
}