using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ReactiveUI;
using Aquila.EntityComponent.Configuration;
using Aquila.Ide.Contracts;
using ComponentEditor = Aquila.EntityComponent.Configuration.ComponentEditor;

namespace Aquila.EntityComponent.IDE
{
    public class ComponentConfigurationItem : ConfigurationItemBase
    {
        public ComponentEditor _editor;

        ObservableCollection<IConfigurationItem> _items;
        public ComponentConfigurationItem(ComponentEditor editor)
        {
            _editor = editor;

            _items = new ObservableCollection<IConfigurationItem>(_editor.Editors.Select(e => new EntityConfigurationItem(e)));

        }
        public override string Caption { get => "Entity"; set { } }

        public override bool CanCreate => true;

        public override ObservableCollection<IConfigurationItem> Childs => _items;

        public override IConfigurationItem Create(string name)
        {
            var obj = _editor.CreateObject();
            obj.Name = name;
            obj.Apply(_editor.Component);
            var item = new EntityConfigurationItem(obj);
            _items.Add(item);
            //this.RaisePropertyChanged("Childs");

            return item;

        }

    }
}
