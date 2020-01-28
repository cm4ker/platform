using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Dock.Model.Controls;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.SimpleIde.Models;
using ReactiveUI;
using ZenPlatform.SimpleIde.Views;

namespace ZenPlatform.SimpleIde.ViewModels
{

    [View(typeof(ConfigurationTreeView))]
    public class ConfigurationTreeViewModel: Tool
    {
        private MDRoot _root;
        public ConfigurationTreeViewModel()
        {
            _root = new MDRoot();
            Configuration = new List<IConfiguratoinItem>()
            {
                new SimpleConfigurationItem("test", new MDRoot())
            };

           

        }


        public void OpenItem(IConfiguratoinItem item)
        {
            OnOpenItem?.Invoke(this, item);

        }

        public event EventHandler<IConfiguratoinItem> OnOpenItem;
        public List<IConfiguratoinItem> Configuration { get; private set; }
    }
}
