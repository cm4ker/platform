using System.Collections.ObjectModel;
using System.Linq;
using Aquila.Ide.Contracts;
using Aquila.SerializableTypeComponent.Configuration.Editors;

namespace Aquila.SerializableTypeComponent.IDE
{
    public class PropertyListConfigurationItem : ConfigurationItemBase
    {
        private TypeEditor _editor;
        private readonly ObservableCollection<IConfigurationItem> _items;
        public PropertyListConfigurationItem(TypeEditor editor)
        {
            
            _editor = editor;
            _items = new ObservableCollection<IConfigurationItem>(_editor.PropertyEditors.Select(p => new PropertyConfigurationItem(p, _editor)));
          
        }
        public override string Caption { get => "Properties"; set { } }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var prop = _editor.CreateProperty();

            prop.Name = name;

            var item = new PropertyConfigurationItem(prop, _editor);
            _items.Add(item);

            //this.RaisePropertyChanged("Childs");

            return item;
        }
    }
}
