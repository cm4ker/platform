using System;
using System.IO;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network
{
    public interface IChannel: IObservable<INetworkMessage>
    {
        bool Running { get; }

        //event Action<Exception> OnError;

        void Send(object message);
        void Start(Stream stream);
        void Stop();

        //void SetHandler(IMessageHandler handler);
    }
}