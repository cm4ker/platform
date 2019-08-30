using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Environment;
using System.Reflection;

namespace ZenPlatform.Core.Test
{
    public class UnitTestServerClient
    {


        [Fact]
        public void Check_assemblyes_update()
        {   

            var serverServices = Initializer.GetServerService();

            var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
            var envManager = serverServices.GetRequiredService<IEnvironmentManager>();

            accessPoint.Start();


            var clientServices = Initializer.GetClientService();

            var client = clientServices.GetRequiredService<PlatformClient>();

            client.Connect(new Settings.DatabaseConnectionSettings() { Address = "127.0.0.1:12345", Database = "test" });
            client.Login("admin", "admin");

            Assert.NotEmpty(client.AssemblyManager.Assemblies);
            //var invoice = 

        }
    }
}
