using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Data;

namespace ZenPlatform.ThinClient
{
    public interface IVisualEnvironment
    {
        public void ShowDock(RuntimeModel rm);

        public void ShowDialog(string xaml, object dataContext);
    }

    public class RuntimeModel
    {
        private readonly string _xaml;
        private readonly object _dataContext;

        public RuntimeModel(string xaml, object dataContext)
        {
            _xaml = xaml;
            _dataContext = dataContext;
        }

        public IControl Run()
        {
            var visual = AvaloniaXamlLoader.Parse(_xaml) as IControl;

            if (visual is IDataContextProvider dc)
            {
                dc.DataContext = _dataContext;
            }

            

            return visual; //form.GetUnderlyingControl();
        }
    }
}