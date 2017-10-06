using System;

namespace RabbitMQClient.Messages
{
    public class AnnulEventArgs
    {
        public Guid MessageId { get; }
        public bool Resend { get; }

        public AnnulEventArgs(Guid messageId, bool resend)
        {
            MessageId = messageId;
            Resend = resend;
        }
    }
}