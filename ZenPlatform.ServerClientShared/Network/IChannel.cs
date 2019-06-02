using System;
using System.IO;

namespace ZenPlatform.ServerClientShared.Network
{
    public interface IChannel
    {
        bool Running { get; }

        event Action<Exception> OnError;

        void Send(object message);
        void Start(Stream stream, IMessageHandler handler);
        void Stop();

        void SetHandler(IMessageHandler handler);
    }
}