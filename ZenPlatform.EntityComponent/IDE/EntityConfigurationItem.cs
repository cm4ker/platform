using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    public class EntityConfigurationItem : IConfigurationItem
    {
        private ObjectEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;
        public EntityConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>();
            _childs.Add(new PropertyListConfigurationItem(_editor));

        }
        public string Caption => _editor.Name;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public ObservableCollection<IConfigurationItem> Childs => _childs;

        public bool CanCreate => false;

        public bool CanDelete => true;

        public bool CanEdit => false;

        public bool CanSearch => true;

        public event PropertyChangedEventHandler PropertyChanged;

        public IConfigurationItem Create(string name)
        {
            throw new NotImplementedException();
        }

        public object GetEditor()
        {
            throw new NotImplementedException();
        }

        public bool Search(string text)
        {
            return _editor.Name.Contains(text);
        }
    }
}
