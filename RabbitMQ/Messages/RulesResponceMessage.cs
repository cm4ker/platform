namespace RabbitMQClient.Messages
{
    public class RulesResponceMessage : Message
    {
        public RulesResponceMessage()
        {
        }

        public RulesResponceMessage(string rulesText)
        {
            RulesText = rulesText;
        }

        public string RulesText { get; set; }
    }
}