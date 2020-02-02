using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.Configuration.Structure
{
    public class ProjectMD
    {
        public ProjectMD()
        {
            ComponentReferences = new List<IComponentRef>();
        }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public List<IComponentRef> ComponentReferences { get; set; }
    }

    public class Project : IProject
    {
        private readonly ProjectMD _md;
        private readonly IInfrastructure _inf;
        private ITypeManager _manager;

        public Project(ProjectMD md, IInfrastructure inf)
        {
            _md = md;
            _inf = inf;
            ProjectId = Guid.NewGuid();
            _manager = inf.TypeManager;
        }

        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        public Guid ProjectId
        {
            get => _md.ProjectId;
            set => _md.ProjectId = value;
        }

        /// <summary>
        /// Имя конфигурации
        /// </summary>
        public string ProjectName
        {
            get => _md.ProjectName;
            set => _md.ProjectName = value;
        }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        public string ProjectVersion
        {
            get => _md.ProjectVersion;
            set => _md.ProjectVersion = value;
        }

        public List<IComponentRef> ComponentReferences
        {
            get => _md.ComponentReferences;
        }


        public IInfrastructure Infrastructure => _inf;
        public ITypeManager TypeManager => _manager;

        /// <summary>
        /// Загрузить концигурацию
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static IProject Load(IFileSystem storage)
        {
            return null;
        }


        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        public void Save(IFileSystem storage)
        {
        }

        public void Load(IInfrastructure inf, IFileSystem fileSystem)
        {
            _manager = inf.TypeManager;
            _manager.LoadSettings(inf.Settings.GetSettings());

            var pkgFolder = "packages";

            foreach (var reference in _md.ComponentReferences)
            {
                var asmPath = Path.Combine(pkgFolder, $"{reference.Name}.dll");

                var asm = Assembly.Load(fileSystem.GetBytes(asmPath) ??
                                        throw new Exception($"Unknown reference {reference.Name}"));

                var loaderType = asm.GetTypes()
                                     .FirstOrDefault(x =>
                                         x.IsPublic && !x.IsAbstract &&
                                         x.GetInterfaces().Contains(typeof(IComponentManager))) ??
                                 throw new Exception("Invalid component");

                var manager = (IComponentManager) Activator.CreateInstance(loaderType);

                manager.Load(this, reference, fileSystem);
            }
        }
    }
}