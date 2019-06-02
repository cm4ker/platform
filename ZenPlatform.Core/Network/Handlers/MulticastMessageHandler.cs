using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class MulticastMessageHandler : IMessageHandler
    {
        private List<IMessageHandler> _list = new List<IMessageHandler>();
        public void Receive(object message, IChannel channel)
        {
            _list.ForEach(h => h.Receive(message, channel));
        }

        public void Subscribe(IMessageHandler handler)
        {
            _list.Add(handler);
        }
    }
}
