using System;
using System.Collections.Generic;

namespace RabbitMQClient
{
    public interface IMessageProcessor
    {
        void Process(Client client, MessageEventArgs e);
        void AfterProcess(Client client);
    }
}