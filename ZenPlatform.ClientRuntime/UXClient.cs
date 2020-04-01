using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ZenPlatform.ClientRuntime.ViewModels;

namespace ZenPlatform.ClientRuntime
{
    public class UXClient
    {
        private static AvaloniaList<Command> _commands;
        private static Window _m;
        private static DockMainWindowViewModel _vm;

        public static void OpenWindow(string xaml, object vm)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var r = new RuntimeModel(xaml, vm);
                _vm.ShowDock(r);
            });
        }

        // public static void OpenWindow(object xaml, object vm)
        // {
        //     var r = new RuntimeModel(xaml, vm);
        //     _vm.ShowDock(r);
        // }

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
                    CanResize = false
                });
            var result = await messageBoxStandardWindow.ShowDialog(_m);
        }

        public AvaloniaList<Command> Commands => _commands;

        public void RegisterCommand(Command cmd)
        {
            _commands.Add(cmd);
        }

        public void RegisterCommand(string name, Action a)
        {
            _commands.Add(new Command {DisplayName = name, Action = a});
        }

        public UXClient(Window mainWindow, DockMainWindowViewModel vm)
        {
            _m = mainWindow;
            _vm = vm;

            _commands = new AvaloniaList<Command>();
            _commands.Add(new Command
            {
                DisplayName = "Open form", Action = () =>
                {
                    var xaml = @"
<UXForm xmlns=""clr-namespace:ZenPlatform.Avalonia.Wrapper;assembly=ZenPlatform.Avalonia.Wrapper"">
  <UXGroup Orientation=""Vertical"">
    <UXTextBox />
    <UXTextBox />
    <UXGroup Orientation = ""Horizontal""> 
        <UXTextBox />
        <UXTextBox />
        <UXCheckBox />
        <UXDatePicker />
        <UXButton />    
    </UXGroup>
  </UXGroup>
</UXForm>";
                    OpenWindow(xaml, null);
                }
            });
            _commands.Add(new Command
                {DisplayName = "Show dialog", Action = () => { UXClient.ShowDialog(); }});
            _commands.Add(new Command {DisplayName = "Some command"});
        }
    }
}