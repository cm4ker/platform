using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network.States
{
    public class EnvironmentManagerObserver : IConnectionObserver<IConnectionContext>
    {
        private IEnvironmentManager _environmentManager;
        private IDisposable _unsbscriber;
        public EnvironmentManagerObserver(IEnvironmentManager environmentManager)
        {
            _environmentManager = environmentManager;
        }
        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestEnvironmentUseNetworkMessage)) || type.Equals(typeof(RequestEnvironmentListNetworkMessage));
        }

        public void OnCompleted(IConnectionContext sender)
        {
            _unsbscriber?.Dispose();
        }

        public void OnError(IConnectionContext sender, Exception error)
        {
            _unsbscriber?.Dispose();
        }

        public void Subscribe(IConnection connection)
        {
            _unsbscriber = connection.Subscribe(this);
        }


        public void OnNext(IConnectionContext context, INetworkMessage value)
        {
            switch (value)
            {
                case RequestEnvironmentUseNetworkMessage msg:
                    msg.UseEnvironment(_environmentManager, (env) =>
                    {
                        _unsbscriber?.Dispose();

                        var state = new AuthenticationObserver(env);
                        state.Subscribe(context.Connection);

                    }).PipeTo(msg.Id, context.Connection.Channel);
                    break;

                case RequestEnvironmentListNetworkMessage msg:
                    msg.GetEnvironmentList(_environmentManager).PipeTo(msg.Id, context.Connection.Channel);
                    break;
            }
        }
    }
}
