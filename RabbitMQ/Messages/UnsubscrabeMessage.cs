namespace RabbitMQClient.Messages
{
    public class UnsubscrabeMessage : Message
    {
        public UnsubscrabeMessage(string channel)
        {
            Body = TransportHelper.PackToArray(channel);
        }
    }
}