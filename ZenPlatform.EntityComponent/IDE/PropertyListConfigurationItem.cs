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

namespace ZenPlatform.EntityComponent.IDE
{
    public class PropertyListConfigurationItem : ReactiveObject, IConfigurationItem
    {
        private ObjectEditor _editor;
        private readonly ObservableCollection<IConfigurationItem> _items;
        public PropertyListConfigurationItem(ObjectEditor editor)
        {
            
            _editor = editor;
            _items = new ObservableCollection<IConfigurationItem>(_editor.Editors.Select(p => new PropertyConfigurationItem(p, _editor)));
          
        }
        public string Caption => "Propertys";

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => false;

        public bool CanCreate => true;

        public bool CanDelete => false;


        public ObservableCollection<IConfigurationItem> Childs => _items;

        public bool CanEdit => false;

        public bool CanSearch => false;

        public IConfigurationItem Create(string name)
        {
            var prop = _editor.CreateProperty();

            prop.Name = name;

            var item = new PropertyConfigurationItem(prop, _editor);
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
