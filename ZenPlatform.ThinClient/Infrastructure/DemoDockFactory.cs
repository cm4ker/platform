using System;
using System.Collections.Generic;
using System.Text;
using Dock.Avalonia.Controls;
using Dock.Avalonia.Editor;
using Dock.Model;
using Dock.Model.Controls;
using ZenPlatform.ThinClient.ViewModels;
using ZenPlatform.ThinClient.ViewModels.Documents;


namespace ZenPlatform.ThinClient.Infrastructure
{
    public class DemoDockFactory : DockFactory
    {
        private object _context;

        public DemoDockFactory()
        {
            _context = new object();
        }

        public DemoDockFactory(object context)
        {
            _context = context;
        }

        public override IDock CreateLayout()
        {
            var document1 = new DocumentViewModel1
            {
                Id = "ПриходнаяНакладная",
                Title = "Документ 001"
            };

            var document2 = new DocumentViewModel1
            {
                Id = "РасходнаяНакладная",
                Title = "Документ 007"
            };

            var mainLayout = new LayoutDock
            {
                Id = "MainLayout",
                Title = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                CurrentView = null,
                Views = CreateList<IView>
                (
                  new DocumentDock
                  {
                      Id = "DocumentsPane",
                      Title = "DocumentsPane",
                      Proportion = double.NaN,
                      CurrentView = document1,
                      Views = CreateList<IView>
                        (
                            document1,
                            document2
                        )
                  }
                  )
            };

            var homeViewModel = new HomeViewModel
            {
                Id = "Main",
                Title = "Main",
                CurrentView = mainLayout,
                Views = CreateList<IView>(mainLayout)
            };

            var root = CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.CurrentView = homeViewModel;
            root.DefaultView = homeViewModel;
            root.Views = CreateList<IView>(homeViewModel);
            root.Top = CreatePinDock();
            root.Top.Alignment = Alignment.Top;
            root.Bottom = CreatePinDock();
            root.Bottom.Alignment = Alignment.Bottom;
            root.Left = CreatePinDock();
            root.Left.Alignment = Alignment.Left;
            root.Right = CreatePinDock();
            root.Right.Alignment = Alignment.Right;

            return root;
        }

        public override void InitLayout(IView layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => _context,
                [nameof(IPinDock)] = () => _context,
                [nameof(ILayoutDock)] = () => _context,
                [nameof(IDocumentDock)] = () => _context,
                [nameof(IToolDock)] = () => _context,
                [nameof(ISplitterDock)] = () => _context,
                [nameof(IDockWindow)] = () => _context,
                [nameof(IDocumentTab)] = () => _context,
                [nameof(IToolTab)] = () => _context,
                ["Document1"] = () => new object(),
                ["MainLayout"] = () => _context,
                ["Home"] = () => layout,
                ["Main"] = () => _context,
                ["Editor"] = () => new LayoutEditor()
                {
                    Layout = layout
                }
            };

            this.HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
