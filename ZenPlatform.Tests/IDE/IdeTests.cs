using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using ZenPlatform.IdeIntegration.Messages.Messages;
using ZenPlatform.IdeIntegration.Server.Infrastructure;

namespace ZenPlatform.Tests.IDE
{
    public class IdeTests
    {
        [Fact]
        public void HelloMessageTest()
        {
            MessageHandlerBackend backend = new MessageHandlerBackend();

            var handler = new ConfigurationMessageHandler();
            var msg = new XCHelloMessage();
            backend.Register(handler);

            backend.Handle(msg);

            Assert.True(msg.Handled);
        }

        [Fact]
        public void TestSimpleServer()
        {
            using (MessageServer ss = new MessageServer())
            {
                ss.RunAsync();


                using (var client = new RequestSocket(">tcp://localhost:5556")) // connect
                {
                    var message = MessagePack.MessagePackSerializer.Typeless.Serialize(
                        new XCHelloMessage()
                        {
                            ComponentName = "test",
                            Handled = true,
                            TextMessage = "My message has been changed"
                        });
                    client.SendFrame(message);
                }

                Task.Delay(5000).Wait();
            }
        }
    }
}