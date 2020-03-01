using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ZenPlatform.ThinClient;

namespace ZenPlatform.UIBuilder
{
    public class App : Application
    {
        public static MainWindow MW;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                MW = new MainWindow();

                desktopLifetime.MainWindow = MW;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}