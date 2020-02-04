using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Ide.Common;

namespace ZenPlatform.SimpleIde.Items
{
    public class MetedataConfigurationItem : IConfigurationItem
    {
        private Guid _metadata_id;
        public MetedataConfigurationItem(Guid metadata_id, TypeManager manager)
        {
            _metadata_id = metadata_id;
        }
        public string Caption => throw new NotImplementedException();

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }


        public bool CanCreate => false;

        public bool CanDelete => false;


        public IEnumerable<IConfigurationItem> Childs => throw new NotImplementedException();

        public bool CanEdit => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Create(string name)
        {
            throw new NotImplementedException();
        }

        public IControl GetEditor()
        {
            throw new NotImplementedException();
        }
    }
}
