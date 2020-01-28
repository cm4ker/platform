using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.Imaging;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.SimpleIde.Models
{
    public class ComponentConfigurationItem : IConfiguratoinItem
    {
        private IComponent _component;
        public ComponentConfigurationItem(IComponent component)
        {
            _component = component;
        }
        public string Content => _component.Name;

        public IBitmap Bitmap => null;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => true;

        public bool HasContext => true;

        public object Context
        {
            get => _component;
            set
            {
                _component = value as IComponent;
            }
        }

        public IEnumerable<IConfiguratoinItem> Childs => null;
    }
}
