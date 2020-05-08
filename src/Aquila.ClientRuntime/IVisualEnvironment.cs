using Avalonia;
using Avalonia.Controls;
using Aquila.Avalonia.Wrapper;
using Aquila.Runtime;

namespace Aquila.ClientRuntime
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
            _xaml = xaml.Replace("LibraryServer", "LibraryClient");
            _dataContext = dataContext;
        }

        public IControl Run()
        {
            var ux = UX.Parse(_xaml);

            var visual = (IControl) ux.GetUnderlyingControl();

            if (visual is IDataContextProvider dc)
            {
                dc.DataContext = _dataContext;
            }

            return visual;
        }
    }
}