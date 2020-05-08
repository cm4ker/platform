using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Network;

namespace Aquila.Core.Tools
{
    public interface IConnectionObserver<TContext>
    {
        void OnCompleted(TContext sender);
        void OnError(TContext sender, Exception error);
        void OnNext(TContext context, INetworkMessage value);
        bool CanObserve(Type type);
    }
}
