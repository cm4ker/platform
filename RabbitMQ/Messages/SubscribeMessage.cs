using QMSSystem;

namespace RabbitMQClient.Messages
{
    public class SubscribeMessage : Message
    {
        public SubscribeMessage(SubscribeParameters parameters)
        {
            Body = TransportHelper.PackToArray(parameters);
        }

        public SubscribeParameters Parameters => TransportHelper.UnpackArray<SubscribeParameters>(Body);
    }
}