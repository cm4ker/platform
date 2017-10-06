using System;
using System.Collections.Generic;
using System.Data;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.EntityObject;
using SqlPlusDbSync.Shared;
using Client = RabbitMQClient.Client;

namespace SqlPlusDbSync.Server.Processors
{
    [MessageProcessor(typeof(DataResponceMessage))]
    public class DataMessageResponceProcessor : IMessageProcessor
    {
        public DataMessageResponceProcessor()
        {
        }

        public void Process(Client client, MessageEventArgs e)
        {
            var msg = e.Message as DataResponceMessage;

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Id = e.Message.Id, From = msg.PointIdFrom, To = context.GeneralPoint, Size = msg.Body.Length, Info = $"Package start version {PlatformHelper.ByteArrayToString(msg.VersionSynced)}" });

                var pointVersion = context.GetSyncVersion(msg.PointIdFrom);
                if (!TransportHelper.ByteArrayCompare(msg.VersionSynced, pointVersion))
                {
                    e.Handled = true;
                    Logger.LogDebug($"Incomming package version: {PlatformHelper.ByteArrayToString(msg.VersionSynced)}. Local version {PlatformHelper.ByteArrayToString(pointVersion)}. Message discarded.");
                    client.SendSplittedMessage(msg.PointIdFrom.ToString(), new DataLoadCompleteMessage(msg.PointIdFrom, context.GeneralPoint));
                    return;
                }

                var c = new Core(context);

                List<DTOObject> entities = null;
                try
                {
                    entities = TransportHelper.UnpackArray<List<DTOObject>>(msg.Body);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Can't read the message", ex);
                    e.Handled = true;
                    return;
                }
                Logger.LogInfo($"Recived objects: {entities.Count}");
                foreach (var entity in entities)
                {
                    Logger.LogDebug($"Loading object: {entity}");
                    try
                    {
                        context.BeginTransaction(IsolationLevel.Snapshot);
                        Logger.LogDebug($"Try to save: {entity}");
                        c.CommitObject(entity);

                        Logger.LogDebug($"Save success: {entity}");
                        Logger.LogDebug($"Try to update version: {PlatformHelper.ByteArrayToString((byte[])entity.Version)}");
                        context.SaveLastChangedVersion((byte[])entity.Version, msg.PointIdFrom);
                        Logger.LogDebug($"Commiting... ");

                        context.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Unhandled error. Transaction rollbacked", ex);
                        context.RollbackTransaction();
                        client.SendMessage(CommonHelper.InfoChannelName, new InfoErrorMessage() { Id = e.Message.Id, From = msg.PointIdFrom, To = context.GeneralPoint, Info = $"Error ex: {ex.Message} stack trace: {ex.StackTrace}" });

                        break;
                    }
                }

                Logger.LogInfo($"Load completed sending...");
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageProcessedMessage() { Id = e.Message.Id });
                client.SendSplittedMessage(msg.PointIdFrom.ToString(), new DataLoadCompleteMessage(msg.PointIdFrom, context.GeneralPoint));
            }
            //WorkingSet.Remove(msg.PointIdFrom);
            e.Handled = true;
        }

        public void AfterProcess(Client client)
        {

        }
    }
}