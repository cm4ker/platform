using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quartz.Util;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client.Processors
{
    [MessageProcessor(typeof(DataLoadCompleteMessage))]
    public class DataLoadCompleteMessageProcessor : IMessageProcessor
    {
        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as DataLoadCompleteMessage;
            using (var context = new AsnaDatabaseContext())
            {
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Id = e.Message.Id, From = msg.From, To = context.GeneralPoint, });
                var c = new Core(context);
                var version = context.GetSyncVersion(msg.From);
                var size = client.SendSplittedMessage(CommonHelper.ServerChannelName, new DataRequestMessage(version, new List<Guid>() { msg.PointId }));
                client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageProcessedMessage() { Id = e.Message.Id, From = msg.From, To = context.GeneralPoint, Size = size, Info = $"Version {PlatformHelper.ByteArrayToString(version)}" });
            }
            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}