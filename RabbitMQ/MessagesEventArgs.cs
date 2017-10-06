using System.Collections.Generic;
using RabbitMQClient.Messages;

namespace RabbitMQClient
{
    public class MessagesEventArgs
    {
        private List<Message> _messages;

        public MessagesEventArgs()
        {
            _messages = new List<Message>();
        }

        public List<Message> Messages => _messages;

        public void Add(Message msg)
        {
            _messages.Add(msg);
        }

        public bool Handled { get; set; }
    }
}