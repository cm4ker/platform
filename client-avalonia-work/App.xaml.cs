using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace client_avalonia_work
{
    public class App : Application
    {
        public override void Initialize()
        {
          
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                desktopLifetime.MainWindow = new MainWindow();

            base.OnFrameworkInitializationCompleted();
        }
    }
}