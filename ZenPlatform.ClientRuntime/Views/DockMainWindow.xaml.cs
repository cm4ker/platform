using Avalonia;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;

namespace ZenPlatform.ClientRuntime.Views
{
    public class DockMainWindow : MetroWindow
    {
        public DockMainWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}