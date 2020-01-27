using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using dnlib.DotNet.Writer;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.TypeSystem;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public class MDRoot : IMDItem
    {
        public MDRoot()
        {
            ComponentReferences = new List<string>();
        }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public List<string> ComponentReferences { get; set; }
    }


    [XmlRoot("Root")]
    public class XCRoot : IXCRoot, IMetaDataItem<MDRoot>
    {
        private ITypeManager _manager;

        public XCRoot()
        {
            ProjectId = Guid.NewGuid();
        }


        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        public Guid ProjectId { get; set; }


        /// <summary>
        /// Имя конфигурации
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        public string ProjectVersion { get; set; }


        public ITypeManager TypeManager => _manager;

        /// <summary>
        /// Загрузить концигурацию
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static IXCRoot Load(IXCConfigurationStorage storage)
        {
            MDManager loader = new MDManager(storage, new TypeManager());

            var root = loader.LoadObject<XCRoot, MDRoot>("root");

            return root;
        }

        /// <summary>
        /// Создать новую концигурацию
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XCRoot Create(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new InvalidOperationException();

            return new XCRoot()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = projectName,
                _manager = new TypeManager()
            };
        }

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        public void Save(IXCConfigurationStorage storage)
        {
            MDManager loader = new MDManager(storage, _manager);

            loader.SaveObject("root", this);
        }

        /// <summary>
        /// Сравнивает две конфигурации
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object CompareConfiguration(IXCRoot another)
        {
            //TODO: Сделать механизм сравнения двух конфигураций
            throw new NotImplementedException();
        }

        public void Initialize(ILoader loader, MDRoot settings)
        {
            ProjectId = settings.ProjectId;
            ProjectName = settings.ProjectName;
            ProjectVersion = settings.ProjectVersion;

            _manager = loader.TypeManager;
            _manager.LoadSettings(loader.Settings.GetSettings());

            foreach (var reference in settings.ComponentReferences)
            {
                loader.LoadObject<MDComponent, MDComponent>(reference);
            }
        }

        public IMDItem Store(IXCSaver saver)
        {
            var settings = new MDRoot()
            {
                ProjectId = ProjectId,
                ProjectName = ProjectName,
                ProjectVersion = ProjectVersion
            };

            foreach (var component in TypeManager.Components)
            {
                saver.SaveObject(component.Info.ComponentName, (MDComponent) component.Metadata);
                settings.ComponentReferences.Add(component.Info.ComponentName);
            }

            return settings;
        }
    }
}