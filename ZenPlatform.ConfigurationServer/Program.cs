using System;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using NetMQ;
using NetMQ.Sockets;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.ConfigurationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new ResponseSocket("@tcp://localhost:5556")) // bind
            using (var client = new RequestSocket(">tcp://localhost:5556"))  // connect
            {
                byte[] data;

                var test = new Random(DateTime.Now.Second).Next(10);

                if (1 % 2 == 0)
                    data = MessagePackSerializer.Typeless.Serialize(
                    new TestMessage()
                    {
                        MsgType = "TestMessage",
                        Hello = "OK",
                        SomeContet = new TestMessage() { Hello = "NASTED" }
                    });
                else
                    data = MessagePackSerializer.Typeless.Serialize(
                        new TestMessage2()
                        {
                            MsgType = "TestMessage2",
                            Hello = "OK",
                            SomeContet = new XCRootViewModel()
                        });

                client.SendFrame(data);

                // Receive the message from the server socket
                byte[] m1 = server.ReceiveFrameBytes();

                Message message = (Message)MessagePack.MessagePackSerializer.Typeless.Deserialize(m1);

                Console.WriteLine("From Client: {0}", message.MsgType);

                Console.ReadKey();
            }
        }
    }

    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public string MsgType { get; set; }
    }

    [MessagePackObject]
    public class TestMessage : Message
    {
        [Key(1)]
        public string Hello { get; set; }

        [Key(2)]
        public TestMessage SomeContet { get; set; }
    }

    [MessagePackObject]
    public class TestMessage2 : Message
    {
        [Key(1)]
        public string Hello { get; set; }

        [Key(2)]
        public XCRootViewModel SomeContet { get; set; }
    }
}
