using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZenPlatform.SimpleIde.Models
{
    public interface IConfiguratoinItem
    {
        string Content { get; }
        IBitmap Bitmap { get; }
        bool IsEnable { get; }
        bool IsExpanded { get; }
        bool CanOpen { get; }
        bool HasContext { get; }
        object Context { get; set; }
        IEnumerable<IConfiguratoinItem> Childs { get; }
    }
}
