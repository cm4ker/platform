using RabbitMQ.Client.Events;

namespace RabbitMQClient.Messages
{
    public delegate void CompleteMessageReciveEventHandler(Message msg, EventingBasicConsumer consumer, BasicDeliverEventArgs args);
}