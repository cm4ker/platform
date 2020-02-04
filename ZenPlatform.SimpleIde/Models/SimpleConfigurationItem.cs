using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.SimpleIde.Models
{
    public class SimpleConfigurationItem : IConfigurationItem
    {
        private List<IConfigurationItem> _childs;

        public event PropertyChangedEventHandler PropertyChanged;

        public SimpleConfigurationItem(string caption)
        {
            Caption = caption;
            _childs = new List<IConfigurationItem>();
        }

        public void AddChild(IConfigurationItem item)
        {
            _childs.Add(item);
        }

        public void Create(string name)
        {
            throw new NotImplementedException();
        }

        public IControl GetEditor()
        {
            throw new NotImplementedException();
        }

        public string Caption { get; }

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => true;

        public IEnumerable<IConfigurationItem> Childs => _childs;

        public bool CanCreate => throw new NotImplementedException();

        public bool CanDelete => throw new NotImplementedException();

        public bool CanEdit => throw new NotImplementedException();
    }
}
