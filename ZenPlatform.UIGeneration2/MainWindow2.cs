using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ZenPlatform.Controls.Avalonia;

namespace ZenPlatform.UIBuilder
{
    public class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            Content = new ObjectPicker();
        }
    }
}