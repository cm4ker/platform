using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
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

                var xaml = @"
<UXForm xmlns=""clr-namespace:ZenPlatform.Avalonia.Wrapper;assembly=ZenPlatform.Avalonia.Wrapper"">
  <UXGroup Orientation=""Vertical"">
    <UXTextBox />
    <UXTextBox />
    <UXGroup Orientation = ""Horizontal""> 
        <UXTextBox />
        <UXTextBox />
        <UXCheckBox />
        <UXDatePicker />
        <UXButton />    
    </UXGroup>
  </UXGroup>
</UXForm>";

                var a = new RuntimeModel(xaml, null);

                view.ShowDock(a);
                view.ShowDock(a);
                view.ShowDock(a);
                view.ShowDock(a);

                // Program.Test();

                desktopLifetime.MainWindow = new DockMainWindow
                {
                    DataContext = view,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}