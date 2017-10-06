using System;

namespace RabbitMQClient.Messages
{
    public abstract class Message
    {
        protected Message()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid From { get; set; }

        public byte[] Body { get; set; }

        public virtual string Channel { get; set; }

        public static Message Unknown()
        {
            return new UnknownMessage();
        }
        public static Message DataMessage()
        {
            return new DataMessage();
        }
    }
}