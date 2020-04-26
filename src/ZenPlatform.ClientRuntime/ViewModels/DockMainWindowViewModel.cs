using System;
using System.Reactive;
using Dock.Model;
using ReactiveUI;
using ZenPlatform.ClientRuntime.Dock;

namespace ZenPlatform.ClientRuntime.ViewModels
{
    public class DockMainWindowViewModel: ViewModelBase, IVisualEnvironment
    {
        public ConfigurationTreeViewModel Configuration { get; set; }

        public MainDockContainer _container;

        
        public ReactiveCommand<Unit, string> SaveProjectCommand { get; private set; }

        public ReactiveCommand<Unit, string> OpenProjectCommand { get; private set; }

      
        public DockMainWindowViewModel()
        {
            // _project = ConfigurationFactory.Create();


            Configuration = new ConfigurationTreeViewModel();

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
        public void ShowDock(RuntimeModel rm)
        {
            _container.Open(rm);
        }

        public void ShowDialog(string xaml, object dataContext)
        {
            throw new NotImplementedException();
        }
    }
}
