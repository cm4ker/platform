using System;
using System.Collections.Generic;
using System.Data;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.EntityObject;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client.Processors
{
    [MessageProcessor(typeof(DataResponceMessage))]
    public class DataMessageResponceProcessor : IMessageProcessor
    {
        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {

            var msg = e.Message as DataResponceMessage;

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Id = e.Message.Id, From = msg.PointIdFrom, To = context.GeneralPoint, Size = msg.Body.Length, Info = $"Package start version {PlatformHelper.ByteArrayToString(msg.VersionSynced)}" });

                var pointVersion = context.GetSyncVersion(msg.PointIdFrom);
                if (!TransportHelper.ByteArrayCompare(msg.VersionSynced, pointVersion))
                {
                    e.Handled = true;
                    return;
                }

                var c = new Core(context);
                var dynObjects = TransportHelper.UnpackArray<List<DTOObject>>(msg.Body);
                Logger.LogDebug($"Recived objects: {dynObjects.Count}");

                foreach (var dynObject in dynObjects)
                {
                    try
                    {
                        Logger.LogDebug($"Save object {dynObject}");
                        context.BeginTransaction(IsolationLevel.Snapshot);
                        c.CommitObject(dynObject);

                        Logger.LogDebug($"Update version..");
                        context.SaveLastChangedVersion((byte[])dynObject.Version, msg.PointIdFrom);

                        Logger.LogDebug($"Commit...");
                        context.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Unhandled error. Transaction rollbacked", ex);
                        context.RollbackTransaction();
                        client.SendMessage(CommonHelper.InfoChannelName, new InfoErrorMessage() { Id = e.Message.Id, From = msg.PointIdFrom, To = context.GeneralPoint, Info = $"Error ex: {ex.Message} stack trace: {ex.StackTrace}" });
                    }
                }
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageProcessedMessage() { Id = e.Message.Id });
            }
            e.Handled = true;
        }
        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}