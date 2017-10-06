using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Transport
{

    public class Client
    {
        private readonly TcpClient _client;
        private Guid _remoteId;

        private int _maxLengthKB = 1024;

        public Client(string host, int port) : this(new TcpClient(host, port))
        {
            _client.ReceiveBufferSize = short.MaxValue;
            _client.SendBufferSize = short.MaxValue;
        }

        public Client(TcpClient client)
        {
            _client = client;
        }

        public Guid RemoteId
        {
            get { return _remoteId; }
            set { _remoteId = value; }
        }

        public bool Connected => _client.Connected;

        public void SendData(string data)
        {
            Guid messageId = Guid.NewGuid();

            var maxCharsPerMessage = _maxLengthKB * 1024 / 7;
            var totalMessages = (int)Math.Ceiling((decimal)data.Length / maxCharsPerMessage);
            for (int i = 0, msgNumber = 0; i < data.Length; i += maxCharsPerMessage, msgNumber++)
            {
                var tinyData = data.Substring(i, Math.Min(i + maxCharsPerMessage, data.Length));

                var package = new Message()
                {
                    Data = tinyData,
                    Id = messageId,
                    MessageNumber = msgNumber + 1,
                    TotalMessages = totalMessages
                };

                var buff = TransportHelper.SerializeBson(package);

                _client.GetStream().Write(buff, 0, buff.Length);
            }
        }

        public string ReadData()
        {
            var buff = new byte[1024];
            return "";
        }

        public void Close()
        {
            _client.Close();
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }

        public string Data { get; set; }

        public int MessageNumber { get; set; }
        public int TotalMessages { get; set; }
    }
}