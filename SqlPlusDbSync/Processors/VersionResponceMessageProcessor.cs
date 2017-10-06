using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client.Processors
{
    [MessageProcessor(typeof(VersionResponceMessage))]
    public class VersionResponceMessageProcessor : IMessageProcessor
    {
        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as VersionResponceMessage;

            client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Channel = e.ChannelName, From = msg.From, To = msg.PointId, Info = "Recive Version responce message" });

            if (msg.PointId == Guid.Empty || msg.Version == null)
            {
                e.Handled = true;
                return;
            }
            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                var c = new Core(context);

                var uncommitedVersion = context.GetLastUncommitedVersion(msg.PointId, Config.Instance.MaxUncommitedTimeInSeconds);

                var objects = c.GetChangedEntityes(msg.Version, new List<Guid> { msg.PointId });
                if (objects.Count > 0)
                {
                    Logger.LogInfo($"Sending to the server {objects.Count} objects.");

                    foreach (var obj in objects)
                    {
                        Logger.LogDebug($"ObjType: {obj.GetType().Name} Version: {PlatformHelper.ByteArrayToString((byte[])obj.Version)}");
                    }

                    var sendMessage =
                        new DataResponceMessage(msg.Body, TransportHelper.PackToArray(objects), msg.PointId)
                        {
                            VersionSynced = (uncommitedVersion is null) ? msg.Version : uncommitedVersion
                        };
                    client.SendSplittedMessage(CommonHelper.ServerChannelName, sendMessage);
                }
                else
                {
                    var version = context.GetSyncVersion(msg.From);
                    var size = client.SendSplittedMessage(CommonHelper.ServerChannelName,
                        new DataRequestMessage(version, new List<Guid> { msg.PointId }));

                    client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageSendMessage()
                    {
                        From = msg.From,
                        To = msg.PointId,
                        Channel = CommonHelper.ServerChannelName,
                        Size = size,
                        Info = "Sending data request message"
                    });
                }
            }

            client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageProcessedMessage() { Info = "Version responce message processed", From = msg.From, To = msg.PointId, Size = 0 });

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}