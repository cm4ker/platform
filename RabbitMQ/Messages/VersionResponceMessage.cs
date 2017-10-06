using System;

namespace RabbitMQClient.Messages
{
    public class VersionResponceMessage : Message
    {

        public VersionResponceMessage()
        {

        }

        public VersionResponceMessage(Guid from, Guid pointId, byte[] version)
        {
            PointId = pointId;
            Version = version;
            From = from;
        }


        public byte[] Version { get; set; }
        public Guid PointId { get; set; }
    }
}