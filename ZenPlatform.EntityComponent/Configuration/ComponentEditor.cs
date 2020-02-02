using System.Collections.Generic;
using System.Reflection;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ComponentEditor
    {
        private readonly Project _proj;
        private readonly IInfrastructure _inf;
        private readonly MDComponent _md;
        private IComponent _com;
        private FileSystemPath _entry;
        private ComponentManager _mrg;

        private List<ObjectEditor> _objs;

        public ComponentEditor(Project proj)
        {
            _proj = proj;
            _inf = proj.Infrastructure;
            _md = new MDComponent();
            _mrg = new ComponentManager();
            _objs = new List<ObjectEditor>();
        }

        // private void CreateFolder()
        // {
        //     var path = FileSystemPath.Root.AppendDirectory("Entity");
        //
        //     if (!_inf.FileSystem.Exists(path))
        //         _inf.FileSystem.CreateDirectory(path);
        // }

        public void Apply()
        {
            _proj.ComponentReferences.Add(new ComponentRef {Entry = Entry.ToString(), Name = "Entity"});
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

        public FileSystemPath Entry
        {
            get => _entry;
            set { _entry = value; }
        }
    }
}