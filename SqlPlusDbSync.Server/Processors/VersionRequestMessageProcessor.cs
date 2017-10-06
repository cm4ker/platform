using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;

namespace SqlPlusDbSync.Server.Processors
{
    [MessageProcessor(typeof(VersionRequestMessage))]
    public class VersionRequestMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(Client client, MessageEventArgs e)
        {
            var msg = e.Message as VersionRequestMessage;

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                byte[] version = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, };
                var from = context.GeneralPoint;
                foreach (var pointId in msg.PointsId)
                {
                    var pointVersion = context.GetSyncVersion(pointId);
                    client.SendSplittedMessage(pointId.ToString(), new VersionResponceMessage(from, pointId, pointVersion));
                }
            }
            e.Handled = true;
        }

        public void AfterProcess(Client client)
        {

        }
    }
}