using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZenPlatform.SimpleIde
{
    public interface IConfiguratoinItem
    {
        string Caption { get; }
        IBitmap Bitmap { get; }
        bool IsEnable { get; }
        bool IsExpanded { get; }
        bool CanOpen { get; }
        bool HasContext { get; }
        object Context { get; set; }
        IEnumerable<IConfiguratoinItem> Childs { get; }
    }
}
