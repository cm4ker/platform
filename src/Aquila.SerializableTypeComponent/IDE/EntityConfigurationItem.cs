using System.Collections.ObjectModel;
using Aquila.Ide.Contracts;
using Aquila.SerializableTypeComponent.Configuration.Editors;

namespace Aquila.SerializableTypeComponent.IDE
{
    public class EntityConfigurationItem : ConfigurationItemBase
    {
        private TypeEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;

        public EntityConfigurationItem(TypeEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>();
            _childs.Add(new PropertyListConfigurationItem(_editor));
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