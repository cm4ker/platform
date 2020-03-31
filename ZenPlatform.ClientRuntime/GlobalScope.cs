using System;
using System.Collections.Generic;
using System.Data;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ClientRuntime
{
    public static class GlobalScope
    {
        private static IPlatformClient _client;
        private static UXInterop _interop;

        public static UXInterop Interop
        {
            get => _interop ?? throw new PlatformNotInitializedException();
            set => _interop = value;
        }

        public static IPlatformClient Client
        {
            get => _client ?? throw new PlatformNotInitializedException();
            set => _client = value;
        }
    }


    public class Command
    {
        public string DisplayName { get; set; }

        public Action Action { get; set; }
    }

    public class UXInterop
    {
        private readonly List<Command> _commands;

        public UXInterop()
        {
            _commands = new List<Command>();
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
                    ClientEnvironment.OpenWindow(xaml, null);
                }
            });
            _commands.Add(new Command
                {DisplayName = "Show dialog", Action = () => { ClientEnvironment.ShowDialog(); }});
            _commands.Add(new Command {DisplayName = "Some command"});
        }

        public IEnumerable<Command> Commands => _commands;

        public void RegisterCommand(Command cmd)
        {
            _commands.Add(cmd);
        }
    }
}