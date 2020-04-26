using Avalonia.Input;
using ReactiveUI;

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