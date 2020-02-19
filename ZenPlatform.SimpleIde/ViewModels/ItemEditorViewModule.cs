using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Ide.Common;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.SimpleIde.ViewModels
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
