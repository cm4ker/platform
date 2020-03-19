using System.IO;
using System.Xml;
using Avalonia;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;

namespace ZenPlatform.ThinClient.Views
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