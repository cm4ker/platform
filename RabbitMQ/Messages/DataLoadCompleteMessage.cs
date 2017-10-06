using System;

namespace RabbitMQClient.Messages
{
    public class DataLoadCompleteMessage : Message
    {
        public DataLoadCompleteMessage()
        {
        }

        public DataLoadCompleteMessage(Guid pointId, Guid pointIdFrom)
        {
            PointId = pointId;
            From = pointIdFrom;
        }


        public Guid PointId { get; set; }
    }
}