using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ReactiveUI;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Configuration.Editors;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    public class PropertyListConfigurationItem : ConfigurationItemBase
    {
        private ObjectEditor _editor;
        private readonly ObservableCollection<IConfigurationItem> _items;
        public PropertyListConfigurationItem(ObjectEditor editor)
        {
            
            _editor = editor;
            _items = new ObservableCollection<IConfigurationItem>(_editor.PropertyEditors.Select(p => new PropertyConfigurationItem(p, _editor)));
          
        }
        public override string Caption { get => "Propertys"; set { } }

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
