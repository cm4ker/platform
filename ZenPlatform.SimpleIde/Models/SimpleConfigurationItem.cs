using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.Imaging;

namespace ZenPlatform.SimpleIde.Models
{
    public class SimpleConfigurationItem : IConfiguratoinItem
    {
        private List<IConfiguratoinItem> _childs;
        public SimpleConfigurationItem(string content, object context)
        {
            Content = content;
            Context = context;
            _childs = new List<IConfiguratoinItem>();
        }

        public void AddChild(IConfiguratoinItem item)
        {
            _childs.Add(item);
        }
        public string Content { get; }

        public IBitmap Bitmap => null;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => true;

        public bool HasContext => true;

        public object Context { get; set; }

        public IEnumerable<IConfiguratoinItem> Childs => _childs;
    }
}
