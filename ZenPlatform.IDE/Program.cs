using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using ZenPlatform.IDE.ViewModels;
using ZenPlatform.IDE.Views;

namespace ZenPlatform.IDE
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
