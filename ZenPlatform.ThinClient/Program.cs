using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Portable.Xaml;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.ThinClient
{
    class Program
    {
        public static void Test()
        {
            UXForm form = new UXForm();
            UXGroup gr = new UXGroup();
            UXTextBox tb = new UXTextBox();
            UXTextBox tb2 = new UXTextBox();

            gr.Childs.Add(tb);
            gr.Childs.Add(tb2);

            form.Content = gr;


            var result = XamlServices.Save(form);


            var xaml =
                @"<UXForm xmlns=""clr-namespace:ZenPlatform.Avalonia.Wrapper;assembly=ZenPlatform.Avalonia.Wrapper"">
  <UXGroup Orientation=""Horizontal"">
    <UXTextBox />
    <UXTextBox />
  </UXGroup>
</UXForm>";

            var a = (UXForm)XamlServices.Parse(xaml);
        }

        public static Assembly ClientMainAssembly { get; set; }

        static void Main(string[] args)
        {

            
            var clientServices = Initializer.GetClientService();
            var platformClient = clientServices.GetRequiredService<IClientPlatformContext>();
            platformClient.Connect(new DatabaseConnectionSettings()
            {
                Address = "127.0.0.1:12345",
                Database = "Library"
            });
            //need check connection

            platformClient.Login("admin", "admin");

            ClientMainAssembly = platformClient.LoadMainAssembly();

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