using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Network;
using ZenPlatform.ServerRuntime;

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
            var serverServices = TestEnvSetup.GetServerServiceWithDatabase(_o);

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
                var service = TestEnvSetup.GetServerService(_o);

                var env = service.GetService<IWorkEnvironment>();

                var session = env.CreateSession(new Anonymous());

                ContextHelper.SetContext(new PlatformContext(session));

                Assert.True(TestExecution());

                Task.Run(() => Assert.True(TestExecution())).Wait();
            }, null);


            Assert.False(TestExecution());
        }

        [Fact]
        public void PlatformReaderTest()
        {
            var c = ExecutionContext.Capture();

            ExecutionContext.Run(c, state =>
            {
                var service = TestEnvSetup.GetServerService(_o);

                var env = service.GetService<IWorkEnvironment>();

                var session = env.CreateSession(new Anonymous());

                ContextHelper.SetContext(new PlatformContext(session));

                PlatformQuery q = new PlatformQuery();
                q.Text = "FROM Entity.Store SELECT A = 1, B = 2, C = Property1";
                var reader = q.ExecuteReader();
                var reader2 = q.ExecuteReader();

                Assert.True(reader.Read());
                var a = reader["A"];
                var b = reader["B"];
                var c = reader["C"];

                Assert.Equal(1, a);
                Assert.Equal(2, b);
                Assert.Equal(100, c);
            }, null);
        }


        public bool TestExecution()
        {
            return ContextHelper.GetContext()?.Session.User.Name == "Anonymous";
        }
    }
}