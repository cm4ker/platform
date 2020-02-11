using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.SimpleIde.Dock;
using ZenPlatform.SimpleIde.ViewModels;

namespace ZenPlatform.SimpleIde
{
    public class LayoutFactory : Factory
    {
        public override IDocumentDock CreateDocumentDock()
        {
            var dock = new DocumentDockContainer();
            dock.Factory = this;
            return dock;
        }

        public override IDock CreateLayout()
        {
            var dock = new ToolsDockContainer();
            dock.Factory = this;
            return dock;
        }

        public override IToolDock CreateToolDock()
        {
            return base.CreateToolDock();
        }

        public override void InitLayout(IDockable layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                
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
