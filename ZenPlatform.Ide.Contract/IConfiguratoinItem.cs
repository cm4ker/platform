
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ZenPlatform.Ide.Contracts
{
    public interface IConfigurationItem: INotifyPropertyChanged
    {
        string Caption { get; }
        bool IsEnable { get; }
        bool IsExpanded { get; }
        bool IsChanged { get; }
        bool CanEdit { get; }
        bool CanCreate { get; }
        bool CanDelete { get; }
        bool CanSearch { get; }
        bool CanSave { get; }


        ObservableCollection<IConfigurationItem> Childs { get; }


        IConfigurationItem Create(string name);

        bool Search(string text);

        void Save();
    }
}
