using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Aquila.EntityComponent.Configuration.Editors;
using Aquila.Ide.Contracts;

namespace Aquila.EntityComponent.IDE
{
    public class CommandListConfigurationItem : ConfigurationItemBase
    {
        private ObjectEditor _editor;
        private readonly ObservableCollection<IConfigurationItem> _items;
        public CommandListConfigurationItem(ObjectEditor editor)
        {

            _editor = editor;
            _items = new ObservableCollection<IConfigurationItem>(_editor.CommandEditors.Select(p => new CommandConfigurationItem(p)));

        }

        public override string Caption { get => "Commands"; set { } }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var prop = _editor.CreateCommand();

            prop.Name = name;

            var item = new CommandConfigurationItem(prop);
            _items.Add(item);


            return item;
        }
    }
}
