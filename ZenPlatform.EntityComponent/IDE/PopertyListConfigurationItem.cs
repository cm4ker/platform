using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    public class PopertyListConfigurationItem : IConfigurationItem
    {
        private ObjectEditor _editor;
        public PopertyListConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
        }
        public string Caption => "Propertys";

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => false;

        public bool CanCreate => true;

        public bool CanDelete => false;


        public IEnumerable<IConfigurationItem> Childs => _editor.Editors.Select(p => new PropertyConfigurationItem(p));

        public bool CanEdit => false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Create(string name)
        {
            var prop = _editor.CreateProperty();

            prop.Name = name;

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
