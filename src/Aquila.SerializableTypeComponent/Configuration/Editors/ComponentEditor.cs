using System.Collections.Generic;
using System.IO;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Ide.Contracts;
using Aquila.SerializableTypeComponent.IDE;
using SharpFileSystem;

namespace Aquila.SerializableTypeComponent.Configuration.Editors
{
    public class ComponentEditor : IComponentEditor
    {
        private readonly IProject _proj;
        private readonly IInfrastructure _inf;
        private readonly MDComponent _md;
        private IComponent _com;
        private FileSystemPath _entry;
        private ComponentManager _mrg;

        private List<TypeEditor> _objs;

        public ComponentEditor(IProject proj)
        {
            _proj = proj;
            _inf = proj.Infrastructure;
            _md = new MDComponent();
            _mrg = new ComponentManager();
            _objs = new List<TypeEditor>();

            _entry = FileSystemPath.Root.AppendFile("SerializableType.xml");
        }

        public ComponentEditor(IProject proj, MDComponent com, IFileSystem fs) : this(proj)
        {
            _md = com;
            LoadExists(fs);
        }

        private void LoadExists(IFileSystem fs)
        {
            foreach (var file in fs.GetEntities(FileSystemPath.Root.AppendDirectory("SerializableType")))
            {
                //var md = fs.Deserialize<MDEntity>(file);
                using (var stream = fs.OpenFile(file, FileAccess.Read))
                {
                    var md = XCHelper.DeserializeFromStream<MDSerializableType>(stream);

                    _objs.Add(new TypeEditor(_inf, md));
                }
            }
        }

        public IComponent Component => _com;

        public IConfigurationItem GetConfigurationTree()
        {
            return new ComponentConfigurationItem(this);
        }

        public void Apply()
        {
            var comRef = new ComponentRef {Entry = Entry.ToString(), Name = "SerializableType"};
            _proj.ComponentReferences.Add(comRef);
            _proj.Attach(comRef, _mrg);
            _proj.RegisterComponentEditor(this);

            _com = _mrg.CreateAndRegisterComponent(_inf, _md);

            foreach (var objectBuilder in _objs)
            {
                objectBuilder.Apply(_com);
            }
        }

        public TypeEditor CreateType()
        {
            var md = new MDSerializableType();
            var r = new TypeEditor(_inf, md);
            _objs.Add(r);
            return r;
        }

        public IEnumerable<TypeEditor> Editors => _objs;

        public FileSystemPath Entry
        {
            get => _entry;
            set { _entry = value; }
        }
    }
}