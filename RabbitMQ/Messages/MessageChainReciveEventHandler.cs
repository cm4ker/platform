using System;

namespace RabbitMQClient.Messages
{
    public delegate void MessageChainReciveEventHandler(Guid messageId);
}