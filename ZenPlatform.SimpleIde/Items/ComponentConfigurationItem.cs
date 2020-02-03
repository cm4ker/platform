using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.Imaging;

namespace ZenPlatform.SimpleIde.Items
{
    public class ComponentConfigurationItem : IConfiguratoinItem
    {
        public string Caption => throw new NotImplementedException();

        public IBitmap Bitmap => throw new NotImplementedException();

        public bool IsEnable => throw new NotImplementedException();

        public bool IsExpanded => throw new NotImplementedException();

        public bool CanOpen => throw new NotImplementedException();

        public bool HasContext => throw new NotImplementedException();

        public object Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IConfiguratoinItem> Childs => throw new NotImplementedException();
    }
}
