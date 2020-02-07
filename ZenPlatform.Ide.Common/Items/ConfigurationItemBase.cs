using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.Ide.Common.Items
{
    public abstract class ConfigurationItemBase : ReactiveObject, IConfigurationItem
    {
        public virtual string Caption => throw new NotImplementedException();

        public virtual bool IsEnable => throw new NotImplementedException();

        public virtual bool IsExpanded { get; set; }

        public virtual bool CanEdit => throw new NotImplementedException();

        public virtual bool CanCreate => throw new NotImplementedException();

        public virtual bool CanDelete => throw new NotImplementedException();


        public bool CanSearch => throw new NotImplementedException();

        public ObservableCollection<IConfigurationItem> Childs => throw new NotImplementedException();

        public abstract IConfigurationItem Create(string name);

        public bool Search(string text)
        {
            throw new NotImplementedException();
        }
    }
}
