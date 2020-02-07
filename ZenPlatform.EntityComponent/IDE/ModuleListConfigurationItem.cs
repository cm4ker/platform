using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using ZenPlatform.EntityComponent.Configuration.Editors;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    
    public class ModuleListConfigurationItem : ConfigurationItemBase
    {
        private ObjectEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;
        public ModuleListConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>();
        }
        public override string Caption { get => "Modules"; set { } }


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
