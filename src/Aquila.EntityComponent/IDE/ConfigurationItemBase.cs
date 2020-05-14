using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Aquila.Ide.Contracts;

namespace Aquila.EntityComponent.IDE
{
    public abstract class ConfigurationItemBase : ReactiveObject, IConfigurationItem
    {
        public abstract string Caption { get; set; }
        public virtual bool IsEnable => true;
        public virtual bool IsExpanded { get; set; }
        public virtual bool IsChanged => false;
        public virtual bool CanEdit => false;
        public virtual bool CanCreate => false;
        public virtual bool CanDelete => false;
        public virtual bool CanSearch => false;
        public virtual bool CanSave => false;

        public virtual ObservableCollection<IConfigurationItem> Childs => null;

        public virtual IConfigurationItem Create(string name) => throw new NotImplementedException();

        public virtual void Save() => throw new NotImplementedException();

        public virtual bool Search(string text) => throw new NotImplementedException();

        public virtual void DiscardChange() => throw new NotImplementedException();

    }
}
