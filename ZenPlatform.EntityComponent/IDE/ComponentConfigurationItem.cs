using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ReactiveUI;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Ide.Contracts;
using ComponentEditor = ZenPlatform.EntityComponent.Configuration.ComponentEditor;

namespace ZenPlatform.EntityComponent.IDE
{
    public class ComponentConfigurationItem : ReactiveObject, IConfigurationItem
    {
        public ComponentEditor _editor;

        ObservableCollection<IConfigurationItem> _items;
        public ComponentConfigurationItem(ComponentEditor editor)
        {
            _editor = editor;

            _items = new ObservableCollection<IConfigurationItem>(_editor.Editors.Select(e => new EntityConfigurationItem(e)));

        }
        public string Caption => "Entity";

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanCreate => true;

        public bool CanDelete => false;


        public ObservableCollection<IConfigurationItem> Childs => _items;

        public bool CanEdit => false;

        public bool CanSearch => false;

        public IConfigurationItem Create(string name)
        {
            var obj = _editor.CreateObject();
            obj.Name = name;
            obj.Apply(_editor.Component);
            var item = new EntityConfigurationItem(obj);
            _items.Add(item);
            //this.RaisePropertyChanged("Childs");

            return item;

        }

        public object GetEditor()
        {
            throw new NotImplementedException();
        }

        public bool Search(string text)
        {
            throw new NotImplementedException();
        }
    }
}
