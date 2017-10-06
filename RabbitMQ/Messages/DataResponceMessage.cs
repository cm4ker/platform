using System;

namespace RabbitMQClient.Messages
{
    public class DataResponceMessage : Message
    {
        private readonly byte[] _versionSynced;

        public DataResponceMessage()
        {
        }

        public DataResponceMessage(byte[] versionSynced, byte[] data, Guid pointIdFrom)
        {

            VersionSynced = versionSynced;
            Body = data;
            PointIdFrom = pointIdFrom;
        }

        public Guid PointIdFrom { get; set; }
        public byte[] VersionSynced { get; set; }
    }
}