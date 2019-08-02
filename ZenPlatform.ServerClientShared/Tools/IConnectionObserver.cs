using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Network;

namespace ZenPlatform.Core.Tools
{
    public interface IConnectionObserver<TContext>
    {
        void OnCompleted(TContext sender);
        void OnError(TContext sender, Exception error);
        void OnNext(TContext context, INetworkMessage value);
        bool CanObserve(Type type);
    }
}
