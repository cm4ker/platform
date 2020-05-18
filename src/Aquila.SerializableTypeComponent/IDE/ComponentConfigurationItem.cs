using System.Collections.ObjectModel;
using System.Linq;
using Aquila.Ide.Contracts;
using Aquila.SerializableTypeComponent.Configuration.Editors;

namespace Aquila.SerializableTypeComponent.IDE
{
    public class ComponentConfigurationItem : ConfigurationItemBase
    {
        public ComponentEditor _editor;

        ObservableCollection<IConfigurationItem> _items;

        public ComponentConfigurationItem(ComponentEditor editor)
        {
            _editor = editor;

            _items = new ObservableCollection<IConfigurationItem>(_editor.Editors.Select(e =>
                new EntityConfigurationItem(e)));
        }

        public override string Caption
        {
            get => "SerializableTypes";
            set { }
        }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var obj = _editor.CreateType();
            obj.Name = name;
            obj.Apply(_editor.Component);
            var item = new EntityConfigurationItem(obj);
            _items.Add(item);
            //this.RaisePropertyChanged("Childs");

            return item;
        }
    }
}