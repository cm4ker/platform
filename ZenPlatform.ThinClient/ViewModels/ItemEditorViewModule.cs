using Dock.Model.Controls;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.ThinClient.ViewModels
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
