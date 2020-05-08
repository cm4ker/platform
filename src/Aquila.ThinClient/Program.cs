using System;
using System.IO;
using System.Reflection;
using System.Security;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Portable.Xaml;
using Aquila.Avalonia.Wrapper;
using Aquila.ClientRuntime;
using Aquila.Compiler.Contracts;
using Aquila.Core.Assemlies;
using Aquila.Core.ClientServices;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Logging;
using Aquila.Core.Network;
using Aquila.Core.Settings;

namespace Aquila.ThinClient
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
                Address = "127.0.0.1:12346",
                Database = "Library"
            });

            if (context.Client.IsConnected) Console.WriteLine("Success connect!");
            else
            {
                Console.WriteLine("Connection has refused!");
                return;
            }

            context.Login("admin", "admin");
            GlobalScope.Client = context.Client;
            ClientMainAssembly = context.LoadMainAssembly();

          


            var result = AppBuilder.Configure<App>()
                .UsePlatformDetect();


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