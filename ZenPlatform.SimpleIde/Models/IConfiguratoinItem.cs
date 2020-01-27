using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZenPlatform.SimpleIde.Models
{
    public interface IConfiguratoinItem: INotifyPropertyChanged
    {
        string Content { get; }
        IBitmap Bitmap { get; }
        bool IsEnable { get; }
        bool IsExpanded { get; }
        bool CanOpen { get; }
        bool HasContext { get; }
        object Context { get; }
        IEnumerable<IConfiguratoinItem> Childs { get; }
    }
}
