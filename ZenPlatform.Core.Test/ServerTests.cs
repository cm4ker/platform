using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Network;

namespace ZenPlatform.Core.Test
{
    public class ServerTests
    {
        private readonly ITestOutputHelper _o;

        public ServerTests(ITestOutputHelper o)
        {
            _o = o;
        }

        [Fact]
        public void TestServer()
        {
            var serverServices = Initializer.GetServerServiceWithDatabase(_o);


            var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
            accessPoint.Start();
            //need check listing


            var environmentManager = serverServices.GetRequiredService<IPlatformEnvironmentManager>();
            Assert.NotEmpty(environmentManager.GetEnvironmentList());
        }
    }
}