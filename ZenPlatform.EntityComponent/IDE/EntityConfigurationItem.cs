using System;
using System.Collections.Generic;
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
        private List<IConfigurationItem> _childs;
        public EntityConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
            _childs = new List<IConfigurationItem>();
            _childs.Add(new PopertyListConfigurationItem(_editor));

        }
        public string Caption => _editor.Name;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public IEnumerable<IConfigurationItem> Childs => _childs;

        public bool CanCreate => false;

        public bool CanDelete => true;

        public bool CanEdit => false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Create(string name)
        {
            throw new NotImplementedException();
        }

        public object GetEditor()
        {
            throw new NotImplementedException();
        }
    }
}
