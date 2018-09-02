using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.Controls.Avalonia
{
    public class ObjectPicker : UserControl
    {
        public ObjectPicker()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
