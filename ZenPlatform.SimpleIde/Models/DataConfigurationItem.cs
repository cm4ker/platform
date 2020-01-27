using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.Imaging;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.SimpleIde.Models
{
    public class DataConfigurationItem : IConfiguratoinItem
    {
        private readonly List<ComponentConfigurationItem> _childs;
        private ITypeManager _typeManager;
        public DataConfigurationItem(ITypeManager typeManager)
        {
            _childs = new List<ComponentConfigurationItem>();
            _typeManager = typeManager;
            foreach (var component in typeManager.Components)
            {
                _childs.Add(new ComponentConfigurationItem(component));
            }
        }
        public string Content => "Data";

        public IBitmap Bitmap => null;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => false;
        public bool HasContext => false;
        public object Context => throw new NotImplementedException();

        public IEnumerable<IConfiguratoinItem> Childs => _childs;
    }
}
