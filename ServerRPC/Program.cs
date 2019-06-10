using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.ServerClientShared.Network;
using System.IO;
using System.Reflection;
using Hyperion.Internal;
using ZenPlatform.AsmInfrastructure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.ServerRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new HyperionSerializer();

            Client client = new Client(new SimpleMessagePackager(new HyperionSerializer()),
                new SimpleConsoleLogger<Client>());

            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
            client.Use("New");

            client.Authentication(new UserPasswordAuthenticationToken("admin", "admin"));

            //Start hack
            Infrastructure.Main(client);

            var asm = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\Debug.dll");
            var type = asm.GetType("CompileNamespace.Test");
            var mi = type.GetMethod("Add");
            mi.Invoke(null, new object[] {2, 3});
            //End hack

            int i = client.Invoke<int, int>(new Route("test"), 44);

            Console.WriteLine($"i = {i}");

            using (var stream = client.InvokeStream(new Route("stream"), 44))
            {
                StreamReader reader = new StreamReader(stream);
                var data = reader.ReadToEnd();

                Console.WriteLine(data);
            }


            client.Close();

            


        }
    }
}