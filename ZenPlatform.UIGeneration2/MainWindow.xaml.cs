using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.UIBuilder
{
    public class MainWindow3 : Window
    {
    }

    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            DataContext = new VM();
        }

        private void InitializeComponent()
        {
           AvaloniaXamlLoader.Load(this);
        }

        public void Changed()
        {
        }
    }
}