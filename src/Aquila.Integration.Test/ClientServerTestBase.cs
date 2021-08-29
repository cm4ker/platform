using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Environment;
using Aquila.Core.Network;

namespace Aquila.Core.Test
{
    public class ClientServerTestBase
    {
        private ITestOutputHelper _testOutput;

        public ClientServerTestBase(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public void InvokeInClientServerContext(InvokeInClientServerContextDelegate action)
        {
            var serverServices = TestEnvSetup.GetServerService(_testOutput);
            var clientServices = TestEnvSetup.GetClientService(_testOutput);


            var environmentManager = serverServices.GetRequiredService<IPlatformInstanceManager>();
            Assert.NotEmpty(environmentManager.GetInstanceList());


            var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
            accessPoint.Start();
            //need check listing

            var platformClient = clientServices.GetRequiredService<ClientPlatformContext>();
            platformClient.Connect(new Settings.DatabaseConnectionSettings()
                {Address = "127.0.0.1:12345", Database = "Library"});
            //need check connection

            platformClient.Login("admin", "admin");
            var assembly = platformClient.LoadMainAssembly();
            Assert.NotNull(assembly);

            action(clientServices, serverServices, platformClient);

            accessPoint.Stop();
        }
    }
}