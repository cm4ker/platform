using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ZenPlatform.SimpleIde.ViewModels;
using ZenPlatform.SimpleIde.Views;

namespace ZenPlatform.SimpleIde
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
