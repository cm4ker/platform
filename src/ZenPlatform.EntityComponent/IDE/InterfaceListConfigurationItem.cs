using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ZenPlatform.EntityComponent.Configuration.Editors;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    public class InterfaceListConfigurationItem : ConfigurationItemBase
    {
        private ObjectEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;

        public InterfaceListConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>(
                _editor.InterfaceEditors.Select(p => new InterfaceConfigurationItem(p)));
        }

        public override string Caption
        {
            get => "Interfaces";
            set { }
        }


        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _childs;


        public override IConfigurationItem Create(string name)
        {
            var module = _editor.CreateInterface();
            var item = new InterfaceConfigurationItem(module);
            _childs.Add(item);

            return item;
        }
    }
}