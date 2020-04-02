using System;
using System.Collections.Generic;
using System.Data;
using Tmds.DBus;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ClientRuntime
{
    public static class GlobalScope
    {
        private static IPlatformClient _client;
        private static UXClient _interop;

        public static UXClient Interop
        {
            get => _interop ?? throw new PlatformNotInitializedException();
            set => _interop = value;
        }

        public static IPlatformClient Client
        {
            get => _client ?? throw new PlatformNotInitializedException();
            set => _client = value;
        }

        public static void AddCommand(Command command)
        {
            _interop.RegisterCommand(command);
        }

        public static void AddCommand(string caption, Action action)
        {
            _interop.RegisterCommand(caption, action);
        }
    }


    public class Command
    {
        public string DisplayName { get; set; }

        public Action Action { get; set; }
    }
}