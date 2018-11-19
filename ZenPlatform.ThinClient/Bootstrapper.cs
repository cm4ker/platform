using Dock.Model;
using DryIoc;
using ZenPlatform.ThinClient.Infrastructure;
using ZenPlatform.ThinClient.ViewModels;

namespace ZenPlatform.ThinClient
{
    public class Bootstrapper
    {
        public static void Init()
        {
            IoC.Container.Register<IMainWindow, MainWindowViewModel>();
            IoC.Container.Register<IDockFactory, StartDockFactory>(made: Made.Of(() => new StartDockFactory()));
        }
    }
}