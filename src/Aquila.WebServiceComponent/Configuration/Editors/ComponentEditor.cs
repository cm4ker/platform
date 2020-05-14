using System.Collections.Generic;
using System.IO;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Ide.Contracts;
using Aquila.WebServiceComponent.IDE;
using SharpFileSystem;

namespace Aquila.WebServiceComponent.Configuration.Editors
{
    public class ComponentEditor : IComponentEditor
    {
        private readonly IProject _proj;
        private readonly IInfrastructure _inf;
        private readonly MDComponent _md;
        private IComponent _com;
        private FileSystemPath _entry;
        private ComponentManager _mrg;

        private List<WebServiceEditor> _objs;

        public ComponentEditor(IProject proj)
        {
            _proj = proj;
            _inf = proj.Infrastructure;
            _md = new MDComponent();
            _mrg = new ComponentManager();
            _objs = new List<WebServiceEditor>();

            _entry = FileSystemPath.Root.AppendFile("WebService.xml");
        }

        public ComponentEditor(IProject proj, MDComponent com, IFileSystem fs) : this(proj)
        {
            _md = com;
            LoadExists(fs);
        }

        private void LoadExists(IFileSystem fs)
        {
            foreach (var file in fs.GetEntities(FileSystemPath.Root.AppendDirectory("WebService")))
            {
                //var md = fs.Deserialize<MDEntity>(file);
                using (var stream = fs.OpenFile(file, FileAccess.Read))
                {
                    var md = XCHelper.DeserializeFromStream<MDWebService>(stream);

                    _objs.Add(new WebServiceEditor(_inf, md));
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
            

            _com = _mrg.CreateAndRegisterComponent(_inf, _md);

            var comRef = new ComponentRef {Entry = Entry.ToString(), Name = _com.Info.ComponentName};
            _proj.ComponentReferences.Add(comRef);
            _proj.Attach(comRef, _mrg);
            _proj.RegisterComponentEditor(this);
            
            foreach (var objectBuilder in _objs)
            {
                objectBuilder.Apply(_com);
            }
        }

        public WebServiceEditor CreateType()
        {
            var md = new MDWebService();
            var r = new WebServiceEditor(_inf, md);
            _objs.Add(r);
            return r;
        }

        public IEnumerable<WebServiceEditor> Editors => _objs;

        public FileSystemPath Entry
        {
            get => _entry;
            set { _entry = value; }
        }
    }
}