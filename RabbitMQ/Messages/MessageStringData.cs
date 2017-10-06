using System.Text;

namespace RabbitMQClient.Messages
{
    public class MessageStringData : Message
    {
        public MessageStringData(string str)
        {
            if (!string.IsNullOrEmpty(str))
                Body = Encoding.UTF8.GetBytes(str);
        }

        public string StringBody
        {
            get { return Encoding.UTF8.GetString(Body); }
        }
    }
}