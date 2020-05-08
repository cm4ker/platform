using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Aquila.Controls.Avalonia
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

        private object _currentObject;

        public int CurrentObjectType { get; set; }
    }
}