using DryIoc;
using Aquila.ServerManagementTool.ViewModels;

namespace Aquila.ServerManagementTool
{
    public class Bootstrapper
    {
        public static void Init()
        {
            IoC.Container.Register<IMainWindow, MainWindowViewModel>();
        }
    }
}