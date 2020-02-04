using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Ide.Common;
using ZenPlatform.Ide.Common.Editors;
using ZenPlatform.Ide.Contracts;
using ZenPlatform.SimpleIde.Models;
using ZenPlatform.Test.Tools;

namespace ZenPlatform.SimpleIde.ViewModels
{
    public class DockMainWindowViewModel: ViewModelBase
    {
        private LayoutFactory LayoutFactory { get; }
        public ConfigurationTreeViewModel Configuration { get; set; }

        private IDocumentDock documentDock;
        public IList<IDockable> LeftTools { get; }
        public IList<IDockable> Documents { get; }

        public ReactiveCommand<IConfigurationItem, IDockable> OpenItemCommand;

        public ReactiveCommand<Unit, IType> AddCommand { get; }

        private Project _project;
        public DockMainWindowViewModel()
        {
            _project = ConfigurationFactory.Create();
            LayoutFactory = new LayoutFactory();
            Documents = new ObservableCollection<IDockable>();
            LeftTools = new ObservableCollection<IDockable>();
            Configuration = new ConfigurationTreeViewModel(_project);

            AddCommand = ReactiveCommand.CreateFromObservable( () => UITypeSelector.SelectType(_project.TypeManager).Handle(Unit.Default));

            Reactive();


            LeftTools.Add(Configuration);

            documentDock = LayoutFactory.CreateDocumentDock();
            documentDock.Id = "DocumentsPane";
            documentDock.Title = "DocumentsPane";
            //    documents.IsCollapsable = false;
            documentDock.Proportion = double.NaN;
            documentDock.VisibleDockables = Documents;

            // ReactiveCommand.Create<Unit, Unit>((u)=> { })

            InitializeDocks();
        }

        public void Reactive()
        {


        }

        

        private void InitializeDocks()
        {


            var treeTool = LayoutFactory.CreateToolDock();
            treeTool.Id = "ConfigurationTreeTool";
            treeTool.Title = "ConfigurationTreeTool";
            treeTool.Proportion = double.NaN;
            //    treeTool.IsCollapsable = false;
            treeTool.VisibleDockables = LeftTools;

            var documents = LayoutFactory.CreateDocumentDock();
            documents.Id = "DocumentsPane";
            documents.Title = "DocumentsPane";
        //    documents.IsCollapsable = false;
            documents.Proportion = double.NaN;
            documents.VisibleDockables = Documents;
            

           var mainLayout = new ProportionalDock();//LayoutFactory.CreateProportionalDock();
           // mainLayout.Proportion = 0.2;
           // mainLayout.IsCollapsable = false;
            mainLayout.Id = "MainLayout";
            mainLayout.Title = "MainLayout";
            mainLayout.Orientation = Orientation.Horizontal;
            
           // mainLayout.ActiveDockable = null;
            mainLayout.VisibleDockables = LayoutFactory.CreateList<IDockable>(
                treeTool,
                LayoutFactory.CreateSplitterDock(),
                documentDock
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


        private IDockable? _layout;
        public IDockable? Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }
    }
}
