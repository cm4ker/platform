using System;
using System.Reactive;
using Dock.Model;
using ReactiveUI;
using SharpFileSystem.FileSystems;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Ide.Contracts;
using ZenPlatform.ThinClient.Dock;

namespace ZenPlatform.ThinClient.ViewModels
{
    public class DockMainWindowViewModel: ViewModelBase
    {
        public ConfigurationTreeViewModel Configuration { get; set; }


        private MainDockContainer _container;

        public ReactiveCommand<IConfigurationItem, IDockable> OpenItemCommand;

        public ReactiveCommand<Unit, string> SaveProjectCommand { get; private set; }

        public ReactiveCommand<Unit, string> OpenProjectCommand { get; private set; }

        private Project _project;

        public DockMainWindowViewModel()
        {
            // _project = ConfigurationFactory.Create();


            Configuration = new ConfigurationTreeViewModel(_project);

            _container = new MainDockContainer();
            



            Reactive();
            _container.OpenTool(Configuration);


        }

        public void Reactive()
        {
            // Configuration.EditItem.Subscribe(item =>
            // {
            //     _container.OpenConfigutaionItem(item);
            // });

            //
            // SaveProjectCommand = ReactiveCommand.CreateFromObservable(
            //     () => Dialogs.OpenDirectory()
            //     );
            // SaveProjectCommand.Subscribe(dir =>
            // {
            //     PhysicalFileSystem fileSystem = new PhysicalFileSystem(dir);
            //     _project.Save(fileSystem);
            // });
            //
            // OpenProjectCommand = ReactiveCommand.CreateFromObservable(
            //     () => Dialogs.OpenDirectory()
            //     );
            // OpenProjectCommand.Subscribe(dir =>
            // {
            //     PhysicalFileSystem fileSystem = new PhysicalFileSystem(dir);
            //     var manager = new MDManager(new TypeManager(), new InMemoryUniqueCounter());
            //     Configuration.Open(Project.Load(manager, fileSystem));
            // });
        }



        public IDockable Layout => _container.Layout;
    }
}
