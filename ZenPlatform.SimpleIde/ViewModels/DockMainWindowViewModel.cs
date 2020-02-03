using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.SimpleIde.Models;

namespace ZenPlatform.SimpleIde.ViewModels
{
    public class DockMainWindowViewModel: ViewModelBase
    {
        private LayoutFactory LayoutFactory { get; }
        public ConfigurationTreeViewModel Configuration { get; set; }

        private IDocumentDock documentDock;
        public IList<IDockable> LeftTools { get; }
        public IList<IDockable> Documents { get; }

        public ReactiveCommand<IConfiguratoinItem, IDockable> OpenItemCommand;
        public DockMainWindowViewModel()
        {
            LayoutFactory = new LayoutFactory();
            Documents = new ObservableCollection<IDockable>();
            LeftTools = new ObservableCollection<IDockable>();
            Configuration = new ConfigurationTreeViewModel();

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
            OpenItemCommand = ReactiveCommand.CreateFromObservable<IConfiguratoinItem, IDockable>(
                (item) =>
                {
                    return Observable.Start(() => new CodeEditorViewModel(new ObjectConfigurationDocument(item)));
                },
                Configuration.WhenAnyValue(c => c.OpenItem).Select(s=>s != null)
            );
            
            OpenItemCommand.Subscribe(d =>
            {
                //Documents.Add(new CodeEditorViewModel(new ObjectConfigurationDocument(new SimpleConfigurationItem("dasdasd", new MDRoot()))));
                Documents.Add(d);
            });
            Configuration.WhenAnyValue(c => c.OpenItem).InvokeCommand(OpenItemCommand);


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

        public ReactiveCommand<Unit, Unit> AddCommand { get; }


        private IDockable? _layout;
        public IDockable? Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }
    }
}
