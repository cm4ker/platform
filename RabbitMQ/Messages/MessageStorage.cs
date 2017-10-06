using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using SqlPlusDbSync.Shared;

namespace RabbitMQClient.Messages
{
    public class MessageStorage
    {
        private List<PartialMessage> _messages;
        private object _lockObject = new object();

        public MessageStorage()
        {
            _messages = new List<PartialMessage>();
        }

        public void Save(PartialMessage message, EventingBasicConsumer consumer, BasicDeliverEventArgs args)
        {
            lock (_lockObject)
            {
                _messages.Add(message);

                if (message.TotalMessages == message.MessageNumber)
                {
                    var msg = GetMessageFromPartial(message.Id);
                    TaskEx.Run(() =>
                    {
                        try
                        {
                            OnMessageRecived?.Invoke(msg, consumer, args);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Error on recive message", ex);
                            _messages.RemoveAll(x => x.Id == message.Id);
                            throw;
                        }
                    });
                }
            }
        }

        public void Remove(Guid messageId)
        {
            _messages.RemoveAll(x => x.Id == messageId);
        }
        public void ClearStorage()
        {
            _messages.Clear();
        }

        public List<PartialMessage> GetPartialMessagesChain(Guid messgeId)
        {
            return _messages.Where(x => x.Id == messgeId).ToList();
        }


        public Message GetMessageFromPartial(Guid messageId)
        {
            var chain = GetPartialMessagesChain(messageId);
            if (!chain.Any()) throw new Exception("Message not found");

            var first = chain.First();
            if (chain.Count != first.TotalMessages) throw new Exception("Message not consistently");

            byte[] data = new byte[chain.Sum(x => x.Body.Length)];
            int index = 0;
            foreach (var message in chain.OrderBy(x => x.MessageNumber))
            {
                message.Body.CopyTo(data, index);
                index += message.Body.Length;
            }
            return TransportHelper.UnpackArray<Message>(data);
        }

        public event CompleteMessageReciveEventHandler OnMessageRecived;
    }
}