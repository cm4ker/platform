using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Ide.Common;
using Aquila.Ide.Contracts;

namespace Aquila.SimpleIde.ViewModels
{
    public class ItemEditorViewModule: Document
    {
        private IConfigurationItem _item;
        public ItemEditorViewModule(IConfigurationItem item)
        {
            _item = item;
            Context = _item;
        }
    }
}
