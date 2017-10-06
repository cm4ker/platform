using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;
using Client = RabbitMQClient.Client;

namespace SqlPlusDbSync.Server.Processors
{
    [MessageProcessor(typeof(DataRequestMessage))]
    public class DataMessageRequestProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(Client client, MessageEventArgs e)
        {
            var msg = e.Message as DataRequestMessage;
            Logger.LogInfo($"Data request message for {msg.Points[0].ToString()}");

            if (msg.Points != null && msg.VersionSynced != null)
                using (AsnaDatabaseContext context = new AsnaDatabaseContext())
                {
                    var c = new Core(context);

                    client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Id = e.Message.Id, From = msg.From, To = context.GeneralPoint, Info = "Requesting data from remote point" });

                    var objects = c.GetChangedEntityes(msg.VersionSynced, msg.Points);

                    Logger.LogInfo($"Sending to the client {objects.Count} objects.");

                    int size = 0;
                    if (objects.Count > 0)
                    {
                        var pack = TransportHelper.PackToArray(objects);

                        size = client.SendSplittedMessage(msg.Points[0].ToString(),
                            new DataResponceMessage(msg.VersionSynced, pack, context.GeneralPoint));
                    }

                    client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageProcessedMessage() { Id = e.Message.Id, From = msg.From, To = context.GeneralPoint, Size = size, Info = $"Send {objects.Count} objects to the remote computer" });
                }

            Logger.LogInfo($"Send exit for {msg.Points[0].ToString()}");

            client.SendSplittedMessage(msg.Points[0].ToString(), new ExitMessage());
            e.Handled = true;
        }

        public void AfterProcess(Client client)
        {

        }
    }
}