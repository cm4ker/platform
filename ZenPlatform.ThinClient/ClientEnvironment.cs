using Avalonia.Controls;
using ZenPlatform.ThinClient.ViewModels;

namespace ZenPlatform.ThinClient
{
    public static class ClientEnvironment
    {
        private static Window _m;
        private static DockMainWindowViewModel _vm;

        public static void OpenWindow(string xaml, object vm)
        {
            var r = new RuntimeModel(xaml, vm);
            _vm.ShowDock(r);
        }


        public static void ShowDialog()
        {
        }


        public static void Init(Window mainWindow, DockMainWindowViewModel vm)
        {
            _m = mainWindow;
            _vm = vm;
        }
    }
}