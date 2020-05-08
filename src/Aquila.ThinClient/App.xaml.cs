using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Aquila.ClientRuntime;
using Aquila.ClientRuntime.ViewModels;
using Aquila.ClientRuntime.Views;
using Aquila.ThinClient.ViewModels;
using Aquila.ThinClient.Views;

namespace Aquila.ThinClient
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