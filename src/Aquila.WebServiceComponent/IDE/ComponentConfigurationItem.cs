using System.Collections.ObjectModel;
using System.Linq;
using Aquila.EntityComponent.IDE;
using Aquila.Ide.Contracts;
using Aquila.WebServiceComponent.Configuration.Editors;

namespace Aquila.WebServiceComponent.IDE
{
    public class ComponentConfigurationItem : ConfigurationItemBase
    {
        public ComponentEditor _editor;

        ObservableCollection<IConfigurationItem> _items;

        public ComponentConfigurationItem(ComponentEditor editor)
        {
            _editor = editor;

            _items = new ObservableCollection<IConfigurationItem>(_editor.Editors.Select(e =>
                new WebServiceConfigurationItem(e)));
        }

        public override string Caption
        {
            get => _editor.Component.Info.ComponentName;
            set { }
        }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var obj = _editor.CreateType();
            obj.Name = name;
            obj.Apply(_editor.Component);
            var item = new WebServiceConfigurationItem(obj);
            _items.Add(item);

            return item;
        }
    }
}