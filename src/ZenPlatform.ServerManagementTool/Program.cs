using System;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using ZenPlatform.ServerManagementTool.ViewModels;

namespace ZenPlatform.ServerManagementTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Init();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}