using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ZenPlatform.ClientRuntime.ViewModels;

namespace ZenPlatform.ClientRuntime
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


        public static async void ShowDialog()
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(
                new MessageBoxStandardParams
                {
                    ShowInCenter = false,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ContentTitle = "This is platform dialog",
                    ContentMessage = "This is content message",
                    Icon = Icon.Info,
                    Style = Style.None,
                    ButtonDefinitions = ButtonEnum.Ok,
                    CanResize = false,
                });
            var result = await messageBoxStandardWindow.ShowDialog(_m);
        }


        public static void Init(Window mainWindow, DockMainWindowViewModel vm)
        {
            _m = mainWindow;
            _vm = vm;
        }
    }
}