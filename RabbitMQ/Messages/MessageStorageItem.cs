namespace RabbitMQClient.Messages
{
    public class MessageStorageItem
    {
        public MessageStorageItem(Message message, string registratorId, MessageType type)
        {
            Message = message;
            RegistratorId = registratorId;
            Type = type;

        }

        public Message Message { get; set; }
        public string RegistratorId { get; set; }
        public MessageType Type { get; set; }
    }
}