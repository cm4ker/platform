﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.Core.Network;
using System.IO;
using System.Reflection;
using ZenPlatform.AsmClientInfrastructure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.ServerRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            

            Client client = new Client(new SimpleConsoleLogger<Client>());


            PlatformClient platformClient = new PlatformClient(null, client);

            platformClient.Connect(new Core.Settings.DatabaseConnectionSettings() { Address = "127.0.0.1:12345", Database = "testdb" });

            platformClient.Login("admin", "admin");


            //platformClient.test();

/*
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
            client.Use("test");

            client.Authentication(new UserPasswordAuthenticationToken("admin", "admin"));
            
            //Start hack
            Infrastructure.Main(client);

            var asm = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\Debug.dll");
            var type = asm.GetType("CompileNamespace.Test");
            var mi = type.GetMethod("Add");
            mi.Invoke(null, new object[] {2, 3});
            //End hack
           
            var service = client.GetService<ITestProxyService>();

            int res = service.Sum(10, 15);
            Console.WriteLine($"res = {res}");

            int i = client.Invoke<int>(new Route("test"), 44);

            Console.WriteLine($"i = {i}");

            using (var stream = client.InvokeStream(new Route("stream"), 44))
            {
                StreamReader reader = new StreamReader(stream);
                var data = reader.ReadToEnd();

                Console.WriteLine(data);
            }


            client.Close();
*/





        }
    }
}