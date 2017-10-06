using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Server.Processors
{
    [MessageProcessor(typeof(RulesRequestMessage))]
    public class RulesRequestMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(Client client, MessageEventArgs e)
        {
            var msg = e.Message as RulesRequestMessage;

            if (msg.ResponceChannel is null)
            {
                e.Handled = true;
                return;
            }

            using (AsnaDatabaseContext context = new AsnaDatabaseContext())
            {
                Core c = new Core(context);
                var rules = c.GetRules();
                client.SendSplittedMessage(msg.ResponceChannel, new RulesResponceMessage(rules));
            }
            e.Handled = true;
        }

        public void AfterProcess(Client client)
        {

        }
    }
}