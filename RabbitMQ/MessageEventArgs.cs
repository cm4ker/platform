using RabbitMQClient.Messages;

namespace RabbitMQClient
{
    public class MessageEventArgs
    {
        private readonly byte[] _rawBody;
        private readonly string _channelName;
        private Message _message;

        public MessageEventArgs(byte[] rawBody, string channelName)
        {
            _rawBody = rawBody;
            _message = TransportHelper.UnpackArray<Message>(_rawBody);
            _channelName = channelName;
        }

        public MessageEventArgs(Message message, string channelName)
        {
            _message = message;
            _channelName = channelName;
        }

        public byte[] RawBody
        {
            get { return _rawBody; }
        }

        public string ChannelName => _channelName;
        public Message Message => _message;



        public bool Handled { get; set; }
    }
}