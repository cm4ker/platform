using System;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Tools;

namespace Aquila.Core.Network.States
{
    public class EnvironmentManagerObserver : IConnectionObserver<IConnectionContext>
    {
        private IPlatformInstanceManager _instanceManager;
        private IDisposable _unsbscriber;

        public EnvironmentManagerObserver(IPlatformInstanceManager instanceManager)
        {
            _instanceManager = instanceManager;
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestEnvironmentUseNetworkMessage)) ||
                   type.Equals(typeof(RequestEnvironmentListNetworkMessage));
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
                    msg.UseEnvironment(_instanceManager, (env) =>
                    {
                        _unsbscriber?.Dispose();

                        var state = new AuthenticationObserver(env);
                        state.Subscribe(context.Connection);
                    }).PipeTo(msg.Id, context.Connection.Channel);
                    break;

                case RequestEnvironmentListNetworkMessage msg:
                    msg.GetEnvironmentList(_instanceManager).PipeTo(msg.Id, context.Connection.Channel);
                    break;
            }
        }
    }
}