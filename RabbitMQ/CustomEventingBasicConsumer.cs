using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQClient
{
    public class CustomEventingBasicConsumer : EventingBasicConsumer
    {
        public CustomEventingBasicConsumer(IModel model) : base(model)
        {
        }

        public string ChannelNameListener { get; set; }
    }
}