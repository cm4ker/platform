using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using dnlib.DotNet;
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
    public class ProjectMD : IEquatable<ProjectMD>
    {
        public ProjectMD()
        {
            ComponentReferences = new List<IComponentRef>();
            ProjectId = Guid.NewGuid();
        }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public List<IComponentRef> ComponentReferences { get; set; }

        public bool Equals(ProjectMD other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProjectId.Equals(other.ProjectId) && ProjectName == other.ProjectName &&
                   ProjectVersion == other.ProjectVersion &&
                   ComponentReferences.SequenceEqual(other.ComponentReferences);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectMD) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectId, ProjectName, ProjectVersion, ComponentReferences);
        }
    }

    public class Project : IProject, IEquatable<IProject>
    {
        private readonly ProjectMD _md;
        private readonly IInfrastructure _inf;
        private ITypeManager _manager;

        private List<IComponentEditor> _editors;
        private Dictionary<IComponentRef, IComponentManager> _managers;

        private static FileSystemPath DefaultPath = FileSystemPath.Root.AppendFile("Project.xml");
        private static FileSystemPath PackagesDirectory = FileSystemPath.Root.AppendDirectory("Packages");

        public static Project Load(IInfrastructure inf, IFileSystem fs)
        {
            var projectMD = fs.Deserialize<ProjectMD>(DefaultPath);
            var prj = new Project(projectMD, inf);
            prj.Load(fs);
            return prj;
        }

        public Project(ProjectMD md, IInfrastructure inf)
        {
            _md = md;
            _inf = inf;
            _manager = inf.TypeManager;
            _managers = new Dictionary<IComponentRef, IComponentManager>();
            _editors = new List<IComponentEditor>();
        }

        public void RegisterComponentEditor(IComponentEditor editor)
        {
            _editors.Add(editor);
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

        public IEnumerable<IComponentEditor> Editors => _editors;


        public IInfrastructure Infrastructure => _inf;
        public ITypeManager TypeManager => _manager;

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="fileSystem"></param>
        public void Save(IFileSystem fileSystem)
        {

            foreach (var mrg in _managers)
            {
                mrg.Value.Save(_inf, mrg.Key, fileSystem);



                if (!fileSystem.Exists(PackagesDirectory))
                    fileSystem.CreateDirectory(PackagesDirectory);

                var asmFile = PackagesDirectory.AppendFile($"{mrg.Key.Name}.dll");

                using (var stream = fileSystem.CreateFile(asmFile))
                {

                    ModuleDefMD module = ModuleDefMD.Load(mrg.Value.GetType().Assembly.Modules.FirstOrDefault());
                    module.Write(stream);
                }




            }

            fileSystem.Serialize(DefaultPath.ToString(), _md);


            
        }

        public void Attach(IComponentRef comRef, IComponentManager mrg)
        {
            _managers.Add(comRef, mrg);
        }

        public void Load(IFileSystem fileSystem)
        {
            _manager = _inf.TypeManager;
           // _manager.LoadSettings(_inf.Settings.GetSettings());


            foreach (var reference in _md.ComponentReferences)
            {
                // Path.Combine(pkgFolder, );

                var asmPath = PackagesDirectory.AppendFile($"{reference.Name}.dll");

                var pdbPath = PackagesDirectory.AppendFile($"{reference.Name}.pdb");

                Assembly asm = null;
                byte[] pdbBytes = null;
                if (fileSystem.Exists(pdbPath))
                {
                    using (var pdbStream = fileSystem.OpenFile(pdbPath, FileAccess.Read))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            pdbStream.CopyTo(memoryStream);
                            pdbBytes = memoryStream.ToArray();
                        }
                    }
                }

                using (var stream = fileSystem.OpenFile(asmPath, FileAccess.Read))
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        if (pdbBytes != null)
                            asm = Assembly.Load(memoryStream.ToArray(), pdbBytes);
                        else
                            asm = Assembly.Load(memoryStream.ToArray());
                    }
                }

                var loaderType = asm.GetTypes()
                                     .FirstOrDefault(x =>
                                         x.IsPublic && !x.IsAbstract &&
                                         x.GetInterfaces().Contains(typeof(IComponentManager))) ??
                                 throw new Exception("Invalid component");

                var manager = (IComponentManager) Activator.CreateInstance(loaderType);
               
                //Attach(reference, manager);

                var editor = manager.Load(this, reference, fileSystem);


                //RegisterComponentEditor(editor);
            }
        }

        public bool Equals(IProject other)
        {
            if (other is Project p)
                return Equals(_md, p._md);

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IProject) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_md, _inf, _manager, _editors, _managers);
        }
    }
}