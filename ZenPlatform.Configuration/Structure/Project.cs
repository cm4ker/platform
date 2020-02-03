using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Structure
{
    public class MDRoot : IMDItem
    {
        public MDRoot()
        {
            ComponentReferences = new List<ComponentRef>();
        }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public List<ComponentRef> ComponentReferences { get; set; }
    }

    public class Project : IProject
    {
        private ITypeManager _manager;

        public Project()
        {
        }

        public Project(ITypeManager manager)
        {
            ProjectId = Guid.NewGuid();
            _manager = manager;
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
        public static IProject Load(IFileSystem storage)
        {
            MDManager loader = new MDManager(storage, new TypeManager());

            var root = loader.LoadObject<Project>("root");

            return root;
        }


        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        public void Save(IFileSystem storage)
        {
            MDManager loader = new MDManager(storage, _manager);
            loader.SaveObject("root", this);
        }

        public void OnLoad(ILoader loader, MDRoot settings)
        {
            ProjectId = settings.ProjectId;
            ProjectName = settings.ProjectName;
            ProjectVersion = settings.ProjectVersion;

            _manager = loader.TypeManager;
            _manager.LoadSettings(loader.Settings.GetSettings());

            var pkgFolder = "packages";

            foreach (var reference in settings.ComponentReferences)
            {
                var asmPath = Path.Combine(pkgFolder, $"{reference.Name}.dll");

                var asm = Assembly.Load(loader.LoadBytes(asmPath) ??
                                        throw new Exception($"Unknown reference {reference.Name}"));

                var loaderType = asm.GetTypes()
                                     .FirstOrDefault(x =>
                                         x.IsPublic && !x.IsAbstract &&
                                         x.GetInterfaces().Contains(typeof(IComponentManager))) ??
                                 throw new Exception("Invalid component");

                var manager = (IComponentManager) Activator.CreateInstance(loaderType);

                manager.Load(reference, loader);
            }
        }
    }
}