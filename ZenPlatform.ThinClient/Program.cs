using System;
using System.IO;
using System.Reflection;
using System.Security;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Portable.Xaml;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Contracts.Network;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.ThinClient
{
    class Program
    {
        public static Assembly ClientMainAssembly { get; set; }

        static void Main(string[] args)
        {
            var clientServices = Initializer.GetClientService();
            var context = clientServices.GetRequiredService<IClientPlatformContext>();
            context.Connect(new DatabaseConnectionSettings()
            {
                Address = "127.0.0.1:12345",
                Database = "Library"
            });

            if (context.Client.IsConnected) Console.WriteLine("Success connect!");
            else
            {
                Console.WriteLine("Connection has refused!");
                return;
            }

            context.Login("admin", "admin");

            ClientMainAssembly = context.LoadMainAssembly();

            var result = AppBuilder.Configure<App>()
                .UsePlatformDetect();

            Infrastructure.Main(context.Client);

            result.StartWithClassicDesktopLifetime(args);
        }
    }


    public class TestClientAssemblyManager : IClientAssemblyManager
    {
        private IAssembly _assembly;

        public TestClientAssemblyManager(IAssembly assembly)
        {
            _assembly = assembly;
        }

        public Stream GetAssembly(string name)
        {
            if (_assembly != null && _assembly.Name == name)
            {
                var stream = new MemoryStream();
                _assembly.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            return new MemoryStream();
        }

        public void UpdateAssemblies()
        {
        }
    }
}