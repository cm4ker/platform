using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class ActionMessageHandler : IMessageHandler
    {
        
        private readonly Action<object, IChannel> _action;

        public static IMessageHandler Create(Action<object, IChannel> action)
        {
            return new ActionMessageHandler(action);
        }

        public ActionMessageHandler(Action<object, IChannel> action)
        {
            _action = action;

        }

        public void Receive(object message, IChannel channel)
        {
            _action(message, channel);
        }
    }
}
