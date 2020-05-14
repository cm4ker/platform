using System.Collections.ObjectModel;
using System.Linq;
using Aquila.Ide.Contracts;
using Aquila.WebServiceComponent.Configuration.Editors;

namespace Aquila.WebServiceComponent.IDE
{
    public class MethodListConfigurationItem : ConfigurationItemBase
    {
        private WebServiceEditor _editor;
        private readonly ObservableCollection<IConfigurationItem> _items;

        public MethodListConfigurationItem(WebServiceEditor editor)
        {
            _editor = editor;
            _items = new ObservableCollection<IConfigurationItem>(
                _editor.MethodEditors.Select(p => new MethodConfigurationItem(p, _editor)));
        }

        public override string Caption
        {
            get => "Methods";
            set { }
        }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var prop = _editor.CreateMethod();

            prop.Name = name;

            var item = new MethodConfigurationItem(prop, _editor);
            _items.Add(item);

            //this.RaisePropertyChanged("Childs");

            return item;
        }
    }
}