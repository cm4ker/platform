using System;
using Aquila.Core.Instance;
using Aquila.Core.Tools;

namespace Aquila.Core.Network.States
{
    public class AuthenticationObserver : IConnectionObserver<IConnectionContext>
    {
        private IDisposable _unsbscriber;
        private AqInstance _instance;

        public AuthenticationObserver(AqInstance instance)
        {
            _instance = instance;
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestAuthenticationNetworkMessage));
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
            if (value is RequestAuthenticationNetworkMessage msg)
            {
                msg.Authentication(_instance.AuthenticationManager, (u) =>
                {
                    ((ServerConnectionContext) context).Session = _instance.CreateSession(u);
                    //context.State = new AuthenticatedState();


                    //context.Connection.Subscribe((InvokeService)_environment.InvokeService);
                    var state = new InvokeObserver(_instance);
                    state.Subscribe(context.Connection);
                    _unsbscriber?.Dispose();
                }).PipeTo(msg.Id, context.Connection.Channel);
            }
        }
    }
}