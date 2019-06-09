using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.ServerClientShared.Network;
using System.IO;
using ZenPlatform.Core.Authentication;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.ServerRPC
{
    class Program
    {


        static void Main(string[] args)
        {
            
            Client client = new Client(new SimpleMessagePackager(new HyperionSerializer()), new SimpleConsoleLogger<Client>());

            client.Open(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
            client.Use("testdb");

            client.Authentication(new UserPasswordAuthenticationToken("admin", "admin"));
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
