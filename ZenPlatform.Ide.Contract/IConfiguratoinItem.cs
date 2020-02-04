
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZenPlatform.Ide.Contracts
{
    public interface IConfigurationItem: INotifyPropertyChanged
    {
        string Caption { get; }
        bool IsEnable { get; }
        bool IsExpanded { get; }
        bool CanEdit { get; }
        bool CanCreate { get; }
        bool CanDelete { get; }

         

        IEnumerable<IConfigurationItem> Childs { get; }


        void Create(string name);

        object GetEditor();

    }
}
