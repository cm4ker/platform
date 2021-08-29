using System.Threading;
using System.Threading.Tasks;
using Aquila.Core;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Test;

namespace Aquila.Core.Test
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
        //     var serverServices = TestEnvSetup.GetServerServiceWithDatabase(_o);
        //
        //     var accessPoint = serverServices.GetRequiredService<IAccessPoint>();
        //     accessPoint.Start();
        //     //need check listing
        //
        //     var environmentManager = serverServices.GetRequiredService<IPlatformEnvironmentManager>();
        //     Assert.NotEmpty(environmentManager.GetEnvironmentList());
        }


    // [Fact]
    // public void PlatformContextTest()
    // {
    //     Assert.False(TestExecution());
    //
    //     var c = ExecutionContext.Capture();
    //
    //     ExecutionContext.Run(c, state =>
    //     {
    //         var service = TestEnvSetup.GetServerService(_o);
    //
    //         var env = service.GetService<IWorkEnvironment>();
    //
    //         var session = env.CreateSession(new Anonymous());
    //
    //         ContextHelper.SetContext(new PlatformContext(session));
    //
    //         Assert.True(TestExecution());
    //
    //         Task.Run(() => Assert.True(TestExecution())).Wait();
    //     }, null);
    //
    //
    //     Assert.False(TestExecution());
    // }
    //
    // [Fact]
    // public void PlatformReaderTest()
    // {
    //     var c = ExecutionContext.Capture();
    //
    //     ExecutionContext.Run(c, state =>
    //     {
    //         var service = TestEnvSetup.GetServerService(_o);
    //
    //         var env = service.GetService<IWorkEnvironment>();
    //
    //         env.Initialize(null);
    //
    //         var session = env.CreateSession(new Anonymous());
    //
    //         ContextHelper.SetContext(new PlatformContext(session));
    //
    //         var store = StoreManager.Create();
    //         store.Property1 = 500;
    //         store.Save();
    //
    //         PlatformQuery q = new PlatformQuery();
    //         q.Text = "FROM Entity.Store WHERE Property1 = @Test SELECT A = 1, B = 2 + 1, C = Property1";
    //         q.SetParameter("Test", 100);
    //
    //         var reader = q.ExecuteReader();
    //         var reader2 = q.ExecuteReader();
    //
    //         Assert.True(reader.Read());
    //         var a = reader["A"];
    //         var b = reader["B"];
    //         var c = reader["C"];
    //
    //         Assert.Equal(1, a);
    //         Assert.Equal(3, b);
    //         Assert.Equal(100, c);
    //     }, null);
    // }
    //
    //
    // public bool TestExecution()
    // {
    //     return ContextHelper.GetContext()?.Session.User.Name == "Anonymous";
    // }
    }
}