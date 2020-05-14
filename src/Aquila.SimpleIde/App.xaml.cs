using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Aquila.SimpleIde.ViewModels;
using Aquila.SimpleIde.Views;

namespace Aquila.SimpleIde
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var view = new DockMainWindowViewModel();

                desktop.MainWindow = new DockMainWindow
                {
                    DataContext = view,
                };
            }

                base.OnFrameworkInitializationCompleted();
        }
    }
}
