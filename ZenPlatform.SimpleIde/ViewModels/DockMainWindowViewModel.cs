using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
using SharpFileSystem;
using SharpFileSystem.FileSystems;

namespace ZenPlatform.SimpleIde.ViewModels
{
    public class DockMainWindowViewModel: ViewModelBase
    {
        private LayoutFactory LayoutFactory { get; }
        public ConfigurationTreeViewModel Configuration { get; set; }

        public IList<IDockable> LeftTools { get; }
        public IList<IDockable> Documents { get; }

        private IDocumentDock _documentDock;

        public ReactiveCommand<IConfigurationItem, IDockable> OpenItemCommand;

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public ReactiveCommand<Unit, string> SaveProjectCommand { get; private set; }

        private Project _project;
        public DockMainWindowViewModel()
        {
            _project = ConfigurationFactory.Create();
            LayoutFactory = new LayoutFactory();
            Documents = new ObservableCollection<IDockable>();
            LeftTools = new ObservableCollection<IDockable>();
            Configuration = new ConfigurationTreeViewModel(_project);



            

            Reactive();


            LeftTools.Add(Configuration);

            // ReactiveCommand.Create<Unit, Unit>((u)=> { })

            InitializeDocks();
        }

        public void Reactive()
        {
            Configuration.EditItem.Subscribe(item =>
            {
                DocumentEditor editor = new DocumentEditor();
                editor.Context = Configuration.SelectedItem;
                Documents.Add(editor);
                _documentDock.ActiveDockable = editor;
            });


            SaveProjectCommand = ReactiveCommand.CreateFromObservable(
                () => Dialogs.OpenDirectory()
                );
            SaveProjectCommand.Subscribe(dir =>
            {
                PhysicalFileSystem fileSystem = new PhysicalFileSystem(dir);
                _project.Save(fileSystem);
            });



        }

        

        private void InitializeDocks()
        {


            var treeTool = LayoutFactory.CreateToolDock();
            treeTool.Id = "ConfigurationTreeTool";
            treeTool.Title = "ConfigurationTreeTool";
            treeTool.Proportion = double.NaN;
            //    treeTool.IsCollapsable = false;
            treeTool.VisibleDockables = LeftTools;

            _documentDock = LayoutFactory.CreateDocumentDock();
            _documentDock.Id = "DocumentsPane";
            _documentDock.Title = "DocumentsPane";
            //    documents.IsCollapsable = false;
            _documentDock.Proportion = double.NaN;
            _documentDock.VisibleDockables = Documents;
            

           var mainLayout = new ProportionalDock();//LayoutFactory.CreateProportionalDock();
           // mainLayout.Proportion = 0.2;
           // mainLayout.IsCollapsable = false;
            mainLayout.Id = "MainLayout";
            mainLayout.Title = "MainLayout";
            mainLayout.Orientation = Orientation.Horizontal;

            var split = LayoutFactory.CreateSplitterDock();
            split.Proportion = 0.3;
            // mainLayout.ActiveDockable = null;
            mainLayout.VisibleDockables = LayoutFactory.CreateList<IDockable>(
                treeTool,
                split,
                _documentDock
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
