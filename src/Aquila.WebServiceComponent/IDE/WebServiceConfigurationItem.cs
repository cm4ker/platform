using System.Collections.ObjectModel;
using Aquila.EntityComponent.IDE;
using Aquila.Ide.Contracts;
using Aquila.WebServiceComponent.Configuration.Editors;

namespace Aquila.WebServiceComponent.IDE
{
    public class WebServiceConfigurationItem : ConfigurationItemBase
    {
        private WebServiceEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;

        public WebServiceConfigurationItem(WebServiceEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>();
            _childs.Add(new ModuleListConfigurationItem(_editor));
        }

        public override string Caption
        {
            get => _editor.Name;
            set { }
        }

        public override bool IsEnable => true;

        public override ObservableCollection<IConfigurationItem> Childs => _childs;

        public override bool CanDelete => true;

        public override bool CanSearch => true;

        public override bool Search(string text)
        {
            return _editor.Name.Contains(text);
        }
    }
}