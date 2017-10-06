using System;
using System.Collections.Generic;

namespace RabbitMQClient.Messages
{
    public class DataRequestMessage : Message
    {


        public DataRequestMessage()
        {
        }

        public DataRequestMessage(byte[] versionSynced, List<Guid> points)
        {
            VersionSynced = versionSynced;
            Points = points;
        }


        public List<Guid> Points { get; set; }
        public byte[] VersionSynced { get; set; }
    }
}