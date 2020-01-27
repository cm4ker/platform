using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.SimpleIde.ViewModels;

namespace ZenPlatform.SimpleIde
{
    public class LayoutFactory : Factory
    {
        private ConfigurationTreeViewModel tree;
        public override IDock CreateLayout()
        {
            return null;
        }

        public override void InitLayout(IDockable layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                ["Tree"] = () =>
                {
                    return tree;
                    }
            };

            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
            };


            base.InitLayout(layout);

        }
    }
}
