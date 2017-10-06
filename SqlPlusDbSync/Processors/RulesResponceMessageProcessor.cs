using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Client.Processors
{
    [MessageProcessor(typeof(RulesResponceMessage))]
    public class RulesResponceMessageProcessor : IMessageProcessor
    {
        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as RulesResponceMessage;

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                var c = new Core(context);
                try
                {
                    c.SaveRules(msg.RulesText);
                    client.SendMessage(CommonHelper.InfoChannelName, new InfoMessageReciveMessage() { Id = e.Message.Id, From = msg.From, To = context.GeneralPoint, Size = msg.RulesText.Length, Info = $"Update rules" });

                }
                catch (Exception ex)
                {
                    Logger.LogError("Unhandled error. Exchanges will go stop.", ex);
                }

            }
            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}