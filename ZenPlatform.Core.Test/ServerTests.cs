using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Environment.Contracts;
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


        [Fact]
        public void PlatformContextTest()
        {
            Assert.False(TestExecution());

            var c = ExecutionContext.Capture();

            ExecutionContext.Run(c, state =>
            {
                var service = Initializer.GetServerService(_o);

                var env = service.GetService<IWorkEnvironment>();

                var session = env.CreateSession(new Anonymous());

                ContextHelper.SetContext(new PlatformContext(session));

                Assert.True(TestExecution());

                Task.Run(() => Assert.True(TestExecution())).Wait();
            }, null);


            Assert.False(TestExecution());
        }


        public bool TestExecution()
        {
            return ContextHelper.GetContext()?.Session.User.Name == "Anonymous";
        }
    }
}