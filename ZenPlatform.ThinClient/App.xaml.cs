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
<UserControl xmlns='https://github.com/avaloniaui'
             xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
             Width='300'>
    <StackPanel> 
        <TextBlock Text='Store caption' />
        <StackPanel Orientation='Horizontal'>
            <TextBlock Text='Prop1'/>
            <TextBox Text='test' Width='200'/>
        </StackPanel>
        <StackPanel Orientation='Horizontal'>
            <TextBlock Text='Prop2'/>
            <TextBox Text='test' Width='200'/>
        </StackPanel>
    </StackPanel>
</UserControl>";

                var a = new RuntimeModel(xaml, null);
                //var co =  //UIBuilder.GetDesktop();
                
                view.ShowDock(a);
                view.ShowDock(a);
                view.ShowDock(a);
                view.ShowDock(a);

                desktopLifetime.MainWindow = new DockMainWindow
                {
                    DataContext = view,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}