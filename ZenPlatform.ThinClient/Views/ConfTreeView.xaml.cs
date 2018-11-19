using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.ThinClient.Views
{
    public class ConfTreeView : UserControl
    {
        public ConfTreeView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
