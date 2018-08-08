using System;
using System.Text;
using MessagePack;
using NetMQ;
using NetMQ.Sockets;

namespace ZenPlatform.ConfigurationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new ResponseSocket("@tcp://localhost:5556")) // bind
            using (var client = new RequestSocket(">tcp://localhost:5556"))  // connect
            {

                var data = MessagePack.MessagePackSerializer.Serialize(new TestMessage() { MsgType = typeof(TestMessage), Hello = "OK", SomeContet = new TestMessage() { Hello = "NASTED" } });

                client.SendFrame(data);

                // Receive the message from the server socket
                byte[] m1 = server.ReceiveFrameBytes();

                Message message = MessagePack.MessagePackSerializer.Deserialize<TestMessage>(m1);

                Console.WriteLine("From Client: {0}", message.MsgType);

                Console.ReadKey();
            }
        }
    }

    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public Type MsgType { get; set; }
    }

    [MessagePackObject]
    public class TestMessage : Message
    {
        [Key(1)]
        public string Hello { get; set; }

        [Key(2)]
        public TestMessage SomeContet { get; set; }
    }
}
