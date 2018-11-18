using System;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Logging.Serilog;
using ZenPlatform.ServerManagementTool.ViewModels;
using ZenPlatform.ServerManagementTool.Views;

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
}