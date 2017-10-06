namespace RabbitMQClient.Messages
{
    public class PartialMessage : Message
    {
        public int MessageNumber { get; set; }
        public int TotalMessages { get; set; }
    }
}