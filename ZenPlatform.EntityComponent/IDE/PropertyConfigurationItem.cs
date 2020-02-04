using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ZenPlatform.Configuration.Common;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.IDE.Editors;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{
    [View(typeof(UiPropertyEditor))]
    class PropertyConfigurationItem : IConfigurationItem
    {
        private PropertyEditor _property;
        public PropertyConfigurationItem(PropertyEditor property)
        {
            _property = property;
        }
        public string Caption => _property.Name;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanCreate => false;

        public bool CanDelete => true;

        public IEnumerable<IConfigurationItem> Childs => null;

        public bool CanEdit => true;

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
