using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;
using ZenPlatform.UIBuilder;

namespace ZenPlatform.ThinClient
{
    class Program
    {
        public static Assembly ClientMainAssembly { get; set; }

        static void Main(string[] args)
        {
            // var clientServices = Initializer.GetClientService();
            // var platformClient = clientServices.GetRequiredService<ClientPlatformContext>();
            // platformClient.Connect(new DatabaseConnectionSettings()
            // {
            //     Address = "127.0.0.1:12345",
            //     Database = "Library"
            // });
            // //need check connection
            //
            // platformClient.Login("admin", "admin");
            //
            // ClientMainAssembly = platformClient.LoadMainAssembly();

            var result = AppBuilder.Configure<App>()
                .UsePlatformDetect();

            result.StartWithClassicDesktopLifetime(args);

            Dispatcher.UIThread.Post(() => { });
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