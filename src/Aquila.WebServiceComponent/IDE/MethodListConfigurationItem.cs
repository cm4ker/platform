using System.Collections.ObjectModel;
using System.Linq;
using Aquila.Component.Shared.IDE;
using Aquila.EntityComponent.IDE;
using Aquila.Ide.Contracts;
using Aquila.WebServiceComponent.Configuration.Editors;

namespace Aquila.WebServiceComponent.IDE
{
    public class ModuleListConfigurationItem : ConfigurationItemBase
    {
        private WebServiceEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;

        public ModuleListConfigurationItem(WebServiceEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>(
                _editor.ModuleEditors.Select(p => new ModuleConfigurationItem(p)));
        }

        public override string Caption
        {
            get => "Modules";
            set { }
        }


        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _childs;


        public override IConfigurationItem Create(string name)
        {
            var module = _editor.CreateModule();
            module.ModuleName = name;
            module.ModuleText = "";
            var item = new ModuleConfigurationItem(module);
            _childs.Add(item);

            return item;
        }
    }
}