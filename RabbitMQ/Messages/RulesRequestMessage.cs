namespace RabbitMQClient.Messages
{
    public class RulesRequestMessage : Message
    {
        public RulesRequestMessage()
        {

        }

        public RulesRequestMessage(string channelResponce)
        {
            ResponceChannel = channelResponce;
        }

        public string ResponceChannel { get; set; }
    }
}