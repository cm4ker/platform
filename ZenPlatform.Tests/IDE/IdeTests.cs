using System;
using System.Net.Security;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using ZenPlatform.Configuration;
using ZenPlatform.IdeIntegration.Messages.Messages;
using ZenPlatform.IdeIntegration.Server.Infrastructure;
using ZenPlatform.Tests.Common;

namespace ZenPlatform.Tests.IDE
{
    public class IdeTests
    {
        [Fact]
        public void HelloMessageTest()
        {
            MessageHandlerBackend backend = new MessageHandlerBackend();

            //var handler = new ConfigurationMessageHandler();
            var msg = new XCHelloMessage();
            //backend.Register(handler);

            backend.Handle(msg);

            Assert.True(msg.Handled);
        }

        [Fact]
        public void TestSimpleServer()
        {
            using (MessageServer ss = new MessageServer())
            {
                ss.RunAsync();
                ss.Register(new ConfigurationMessageHandler(Factory.CreateExampleConfiguration()));

                using (var client = new RequestSocket(">tcp://localhost:5556")) // connect
                {
                    var requestFrame = MessagePack.MessagePackSerializer.Typeless.Serialize(
                        new XCTreeRequestMessage()
                        {
                            ItemType = XCNodeKind.Root,
                        });
                    client.SendFrame(requestFrame);
                    var responceFrame = client.ReceiveFrameBytes();
                    var responce = MessagePack.MessagePackSerializer.Typeless.Deserialize(responceFrame);

                    var msg = Assert.IsType<XCTreeResponceMessage>(responce);
                    Assert.Equal(1, msg.Items.Count);
                }

                Task.Delay(5000).Wait();
            }
        }
    }
}