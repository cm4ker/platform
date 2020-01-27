using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.SimpleIde
{
    public class PropertyGrid : UserControl
    {

        private Grid _propertyName;
        private Grid _propertyValue;
        private object _selectedObject;
        public PropertyGrid()
        {
            this.InitializeComponent();

            _selectedObject = null;
            _propertyName = this.FindControl<Grid>("PropertyName");
            _propertyValue = this.FindControl<Grid>("PropertyValue");


        }

        void OnSelectedObjectChange(object obj)
        {
            var props = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public);



            foreach (var prop in props)
            {
                TextBlock block = new TextBlock();
                block.Text = prop.Name;
             
            }
        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public object SelectedObject
        {
            get => _selectedObject;
            set {
                _selectedObject = value;
                OnSelectedObjectChange(value);
            }
        }
    }
}
