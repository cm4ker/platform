using System;
using System.Threading.Tasks;
using Xunit;
using ZenPlatform.Core.RPC;

namespace Core.RPC.Tests
{
    public class ServerTests
    {
        private Server server;
        private Client client;
        private ServiceClass service = new ServiceClass();
        private ServiceClassImpl serviceImpl = new ServiceClassImpl();

        public ServerTests()
        {
            
            server = ServerBuilder.CreateBuilder()
                .AddHost("localhost", 5544)
                .SetSerialiser(new DeflateJSONSerializer())
                .Build();




            server.AddService(typeof(IServiceInterface), serviceImpl.GetType());

            //server.AddService(service.GetType());

            server.Start();

            client = ClientBuilder.CreateBuilder()
                        .SetSerialiser(new DeflateJSONSerializer())
                        .SetServer("localhost:5544")
                        .Build();
                        
        }

        [Fact]
        public void ChangeThisTest()
        {

            try
            {
                server.AddService(service.GetType());
            } catch (Exception ex)
            {

            }
            var t = Task.Run(async () =>
            {
                
                    await Task.Delay(500);

                    var sum = await client.Invoke(RouteFactory.Instance.GetRoute<ServiceClass>("Sum"), service, new object[] { 10, 20 });

                    Assert.Equal(30, service.value);
            });


        }


        [Fact]
        public void InterfaceImplTest()
        {
            server.AddService(typeof(IServiceInterface), serviceImpl.GetType());

            var t = Task.Run(async () =>
            {

                await Task.Delay(500);

                var sum = await client.Invoke(RouteFactory.Instance.GetRoute<ServiceClassImpl>("Sum"), service, new object[] { 10, 20 });

                Assert.Equal(30, sum);
            });


        }
    }
}
