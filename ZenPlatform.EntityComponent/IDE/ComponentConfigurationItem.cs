using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Ide.Contracts;
using ComponentEditor = ZenPlatform.EntityComponent.Configuration.ComponentEditor;

namespace ZenPlatform.EntityComponent.IDE
{
    public class ComponentConfigurationItem : IConfigurationItem
    {
        public ComponentEditor _editor;
        public ComponentConfigurationItem(ComponentEditor editor)
        {
            _editor = editor;

            
        }
        public string Caption => "Entity";

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanCreate => true;

        public bool CanDelete => false;


        public IEnumerable<IConfigurationItem> Childs => _editor.Editors.Select(e => new EntityConfigurationItem(e));

        public bool CanEdit => false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Create(string name)
        {
            var obj = _editor.CreateObject();
            obj.Name = name;

            NotifyPropertyChanged("Childs");

        }

        public object GetEditor()
        {
            throw new NotImplementedException();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
