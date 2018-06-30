using Avalonia.Input;

namespace ZenPlatform.ServerManagementTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindow
    {
        public MainWindowViewModel()
        {
        }


        public string Text => "Hello ";
    }

    public interface IMainWindow
    {
    }
}