using System.Collections.Generic;
using System.Reflection;
using SharpFileSystem;
using Aquila.Configuration;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data;
using Aquila.EntityComponent.IDE;
using Aquila.Ide.Common;
using Aquila.Ide.Contracts;
using Aquila.EntityComponent.Configuration.Editors;
using System.IO;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.EntityComponent.Configuration
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
            
            _entry = FileSystemPath.Root.AppendFile("Entity.xml");
           
        }

        public ComponentEditor(IProject proj, MDComponent com, IFileSystem fs) : this(proj)
        {
            _md = com;
            LoadExists(fs);
        }

        private void LoadExists(IFileSystem fs)
        {
            foreach (var file in fs.GetEntities(FileSystemPath.Root.AppendDirectory("Entity")))
            {
               //var md = fs.Deserialize<MDEntity>(file);
                using (var stream = fs.OpenFile(file, FileAccess.Read))
                {
                    var md =  XCHelper.DeserializeFromStream<MDEntity>(stream);

                    _objs.Add(new ObjectEditor(_inf, md));
                }

            }
        }

        public IComponent Component => _com;

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
            var md = new MDEntity();
            var r = new ObjectEditor(_inf, md);
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