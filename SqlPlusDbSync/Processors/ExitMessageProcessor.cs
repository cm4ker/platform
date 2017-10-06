using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client.Processors
{
    [MessageProcessor(typeof(ExitMessage))]
    public class ExitMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {

            var msg = e.Message as ExitMessage;
            e.Handled = true;

            if (Guid.TryParse(e.ChannelName, out var channelId))
                client.SendMessage(CommonHelper.InfoChannelName, new InfoDisconnectMessage { From = channelId });
            Logger.LogInfo("Start unsubscribe");
            client.Unsubscribe(e.ChannelName);
           

        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}