using System;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Logging.Serilog;
using DryIoc;
using ZenPlatform.ServerManagementTool.ViewModels;

namespace ZenPlatform.ServerManagementTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Init();

            BuildAvaloniaApp().Start<MainWindow>(IoC.Resolve<IMainWindow>);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }

    public class Bootstrapper
    {
        public static void Init()
        {
            IoC.Container.Register<IMainWindow, MainWindowViewModel>();
        }
    }
}