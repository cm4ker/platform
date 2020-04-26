using DryIoc;
using ZenPlatform.ServerManagementTool.ViewModels;

namespace ZenPlatform.ServerManagementTool
{
    public class Bootstrapper
    {
        public static void Init()
        {
            IoC.Container.Register<IMainWindow, MainWindowViewModel>();
        }
    }
}