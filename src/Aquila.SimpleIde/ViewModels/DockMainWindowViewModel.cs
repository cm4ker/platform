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
using Aquila.Configuration.Structure;
using Aquila.Ide.Common;
using Aquila.Ide.Common.Editors;
using Aquila.Ide.Contracts;
using Aquila.Test.Tools;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using Aquila.SimpleIde.Dock;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Configuration.Storage;
using Aquila.Configuration;
using Aquila.Compiler.Platform;
using Aquila.Compiler.Dnlib;

namespace Aquila.SimpleIde.ViewModels
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