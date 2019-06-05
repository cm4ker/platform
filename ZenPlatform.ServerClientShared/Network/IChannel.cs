using System;
using System.IO;
using ZenPlatform.Core.Authentication;


namespace ZenPlatform.ServerClientShared.Network
{
    public interface IChannel
    {
        bool Running { get; }

        event Action<Exception> OnError;

        void Send(object message);
        void Start(IConnection connection);
        void Stop();

        void SetHandler(IMessageHandler handler);
    }
}