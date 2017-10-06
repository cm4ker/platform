using System;
using System.Collections.Generic;
using RabbitMQClient;
using RabbitMQClient.Messages;

namespace SqlPlusDbSync.Live.Processors
{
    [MessageProcessor(typeof(InfoConnectMessage))]
    public class InfoConnectMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as InfoConnectMessage;

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }

    [MessageProcessor(typeof(InfoDisconnectMessage))]
    public class InfoDisconnectMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as InfoDisconnectMessage;

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }

    [MessageProcessor(typeof(InfoMessageReciveMessage))]
    public class InfoMessageReciveMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as InfoMessageReciveMessage;

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }

    [MessageProcessor(typeof(InfoErrorMessage))]
    public class InfoErrorMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as InfoErrorMessage;

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }

    [MessageProcessor(typeof(InfoMessageSendMessage))]
    public class InfoMessageSendMessageProcessor : IMessageProcessor
    {
        public HashSet<Guid> WorkingSet { get; set; }

        public void Process(RabbitMQClient.Client client, MessageEventArgs e)
        {
            var msg = e.Message as InfoMessageSendMessage;

            e.Handled = true;
        }

        public void AfterProcess(RabbitMQClient.Client client)
        {

        }
    }
}