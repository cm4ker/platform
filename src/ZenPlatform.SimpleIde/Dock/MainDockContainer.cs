using Dock.Model;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ZenPlatform.Ide.Contracts;
using ZenPlatform.SimpleIde.ViewModels;

namespace ZenPlatform.SimpleIde.Dock
{
    public class MainDockContainer: ReactiveObject
    {
        private static MainDockContainer _instance = null;
        public static MainDockContainer Instance { 
            get
            {
                if (_instance == null)
                    _instance = new MainDockContainer();
                return _instance;
            }
        }

        private ToolsDockContainer _leftTools;
        private DocumentDockContainer _documents;

        private IDockable _layout;
        public IDockable Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        private LayoutFactory LayoutFactory { get; }

        public DocumentView ActiveDocument => (DocumentView)_documents.ActiveDockable;

        public MainDockContainer()
        {
            LayoutFactory = new LayoutFactory();
            Initialize();
        }

        public void OpenConfigutaionItem(IConfigurationItem item)
        {
            var view = new DocumentView(item);
            view.Factory = LayoutFactory;
            _documents.OpenDocument(view);
        }

        public void OpenTool(IDockable tool)
        {
            _leftTools.OpenTool(tool);
        }

        private void Initialize()
        {

            _leftTools = new ToolsDockContainer();
            _leftTools.Factory = LayoutFactory;
            _documents = new DocumentDockContainer();
            _documents.Factory = LayoutFactory;


            var mainLayout = LayoutFactory.CreateProportionalDock();
                                                    // mainLayout.Proportion = 0.2;
            mainLayout.IsCollapsable = false;
            mainLayout.Id = "MainLayout";
            mainLayout.Title = "MainLayout";
            mainLayout.Orientation = Orientation.Horizontal;
            
            var split = LayoutFactory.CreateSplitterDock();
            split.Proportion = 0.3;
            
            _leftTools.Proportion = 0.3;
            
            // mainLayout.ActiveDockable = null;
            mainLayout.VisibleDockables = LayoutFactory.CreateList<IDockable>(
                _leftTools,
                split,
                _documents
            );



            var root = LayoutFactory.CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.IsCollapsable = false;
            root.ActiveDockable = mainLayout;
            root.DefaultDockable = mainLayout;
            root.VisibleDockables = LayoutFactory.CreateList<IDockable>(mainLayout);
            root.Top = LayoutFactory.CreatePinDock();
            root.Top.Alignment = Alignment.Top;
            root.Bottom = LayoutFactory.CreatePinDock();
            root.Bottom.Alignment = Alignment.Bottom;
            root.Left = LayoutFactory.CreatePinDock();
            root.Left.Alignment = Alignment.Left;
            root.Right = LayoutFactory.CreatePinDock();
            root.Right.Alignment = Alignment.Right;


            LayoutFactory.InitLayout(root);
            Layout = root;
        }
        
    }
}
