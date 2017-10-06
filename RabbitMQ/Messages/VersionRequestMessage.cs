using System;
using System.Collections.Generic;

namespace RabbitMQClient.Messages
{
    public class VersionRequestMessage : Message
    {

        public VersionRequestMessage()
        {
        }

        public VersionRequestMessage(List<Guid> pointsId)
        {
            PointsId = pointsId;
        }

        public List<Guid> PointsId { get; set; }

    }
}