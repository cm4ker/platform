using System.Collections.Generic;
using System.Reflection;
using SharpFileSystem;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.EntityComponent.IDE;
using ZenPlatform.Ide.Common;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ComponentEditor : IComponentEditor
    {
        private readonly IProject _proj;
        private readonly IInfrastructure _inf;
        private readonly MDComponent _md;
        private IComponent _com;
        private FileSystemPath _entry;
        private ComponentManager _mrg;

        private List<ObjectEditor> _objs;

        public ComponentEditor(IProject proj)
        {
            _proj = proj;
            _inf = proj.Infrastructure;
            _md = new MDComponent();
            _mrg = new ComponentManager();
            _objs = new List<ObjectEditor>();
            
            _entry = FileSystemPath.Root.AppendFile("Entity");
           
        }

        public ComponentEditor(IProject proj, MDComponent com, IFileSystem fs) : this(proj)
        {
            _md = com;
            LoadExists(fs);
        }

        private void LoadExists(IFileSystem fs)
        {
            foreach (var file in fs.GetEntities(FileSystemPath.Parse("/Entity/")))
            {
                var md = fs.Deserialize<MDEntity>(file);
                _objs.Add(new ObjectEditor(_inf, md));
            }
        }

        // private void CreateFolder()
        // {
        //     var path = FileSystemPath.Root.AppendDirectory("Entity");
        //
        //     if (!_inf.FileSystem.Exists(path))
        //         _inf.FileSystem.CreateDirectory(path);
        // }

        public IConfigurationItem GetConfigurationTree()
        {
            return new ComponentConfigurationItem(this);
        }

        public void Apply()
        {
            var comRef = new ComponentRef {Entry = Entry.ToString(), Name = "Entity"};
            _proj.ComponentReferences.Add(comRef);
            _proj.Attach(comRef, _mrg);
            _proj.RegisterComponentEditor(this);

            _com = _mrg.CreateAndRegisterComponent(_inf, _md);

            foreach (var objectBuilder in _objs)
            {
                objectBuilder.Apply(_com);
            }
        }

        public ObjectEditor CreateObject()
        {
            var r = new ObjectEditor(_inf);
            _objs.Add(r);
            return r;
        }

        public IEnumerable<ObjectEditor> Editors => _objs;

        public FileSystemPath Entry
        {
            get => _entry;
            set { _entry = value; }
        }
    }
}