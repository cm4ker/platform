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
using ZenPlatform.Test.Tools;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using ZenPlatform.SimpleIde.Dock;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Compiler.Dnlib;

namespace ZenPlatform.SimpleIde.ViewModels
{
    public class DockMainWindowViewModel : ViewModelBase
    {
        public ConfigurationTreeViewModel Configuration { get; set; }


        private MainDockContainer _container;

        public ReactiveCommand<IConfigurationItem, IDockable> OpenItemCommand;

        public ReactiveCommand<Unit, string> SaveProjectCommand { get; private set; }

        public ReactiveCommand<Unit, string> OpenProjectCommand { get; private set; }

        private Project _project;

        public DockMainWindowViewModel()
        {
            _project = ConfigurationFactory.Create();


            Configuration = new ConfigurationTreeViewModel(_project);

            _container = MainDockContainer.Instance;

            Reactive();
            _container.OpenTool(Configuration);
        }

        public void Reactive()
        {
            Configuration.EditItem.Subscribe(item => { _container.OpenConfigutaionItem(item); });


            SaveProjectCommand = ReactiveCommand.CreateFromObservable(
                () => Dialogs.OpenDirectory()
            );
            SaveProjectCommand.Subscribe(dir =>
            {
                PhysicalFileSystem fileSystem = new PhysicalFileSystem(dir);
                _project.Save(fileSystem);
            });


            OpenProjectCommand = ReactiveCommand.CreateFromObservable(
                () => Dialogs.OpenDirectory()
            );

            OpenProjectCommand.Subscribe(dir =>
            {
                PhysicalFileSystem fileSystem = new PhysicalFileSystem(dir);
                var manager = new MDManager(new TypeManager(), new InMemoryUniqueCounter());
                var prj = Project.Load(manager, fileSystem);
                Configuration.Open(prj);
            });
        }


        public IDockable Layout => _container.Layout;
    }
}