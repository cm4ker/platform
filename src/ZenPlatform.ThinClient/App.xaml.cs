using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ZenPlatform.ClientRuntime;
using ZenPlatform.ClientRuntime.ViewModels;
using ZenPlatform.ClientRuntime.Views;
using ZenPlatform.ThinClient.ViewModels;
using ZenPlatform.ThinClient.Views;

namespace ZenPlatform.ThinClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var view = new DockMainWindowViewModel();

                desktopLifetime.MainWindow = new DockMainWindow
                {
                    DataContext = view,
                };

                GlobalScope.Interop = new UXClient(desktopLifetime.MainWindow, view);

                Program.ClientMainAssembly.GetType("EntryPoint")?.GetMethod("Main")
                    ?.Invoke(null, new[] {new object[] { }});
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}