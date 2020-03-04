using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

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
                var window = new MainWindow();
                var panel = window.FindControl<Panel>("Panel");

                var initializer = Program.ClientMainAssembly.GetType("ZenPlatform.ThinClient.FakeAssembly.UIBuilder");
                var metohd = initializer.GetMethod("GetDesktop");

                var result = metohd.Invoke(null, null);

                if (result != null && result is IControl c)
                    panel.Children.Add(c);

                desktopLifetime.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}