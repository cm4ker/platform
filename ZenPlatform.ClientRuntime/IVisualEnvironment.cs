using Avalonia;
using Avalonia.Controls;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Runtime;

namespace ZenPlatform.ClientRuntime
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
        private object _obj;

        public RuntimeModel(string xaml, object dataContext)
        {
            _xaml = xaml.Replace("LibraryServer", "LibraryClient");
            _dataContext = dataContext;
        }

        public RuntimeModel(object obj, object dataContext)
        {
            _obj = obj;
            _dataContext = dataContext;
        }

        public IControl Run()
        {
            var ux = (_xaml == null) ? (UXElement) _obj : UX.Parse(_xaml);

            var visual = (IControl) ux.GetUnderlyingControl();

            if (visual is IDataContextProvider dc)
            {
                dc.DataContext = _dataContext;
            }

            return visual;
        }
    }
}