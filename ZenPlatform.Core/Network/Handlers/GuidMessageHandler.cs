using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public abstract class MessageHandler : IMessageHandler
    {
        public abstract void Receive(object message, IChannel channel);
    }





    public abstract class GuidMessageHandler : MessageHandler
    {
        private Dictionary<Guid, IMessageHandler> _handlers = new Dictionary<Guid, IMessageHandler>();


        protected void AddHandlerByGuid(Guid guid, IMessageHandler handler)
        {

            _handlers.Add(guid, handler);
        }

        protected void DelHandlerByGuid(Guid guid)
        {
            _handlers.Remove(guid);
        }

        public override sealed void Receive(object message, IChannel channel)
        {
            var msg = message as INetworkMessage;
            bool processed = false;
            if (_handlers.TryGetValue(msg.Id, out var handler))
            {
                handler.Receive(msg, channel);
                processed = true;
            }
            ReceiveMessage(message, channel, processed);
        }


        public abstract void ReceiveMessage(object message, IChannel channel, bool processed);


    }
}
