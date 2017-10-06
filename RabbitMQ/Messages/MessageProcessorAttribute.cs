using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQClient.Messages
{

    public class MessageProcessorAttribute : Attribute
    {
        private readonly Type[] _messageType;

        public MessageProcessorAttribute(params Type[] messageType)
        {
            _messageType = messageType;
        }

        public IEnumerable<Type> MessageTypes => _messageType;
    }
}
