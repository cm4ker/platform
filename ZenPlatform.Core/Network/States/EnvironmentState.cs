using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.Core.Network.States
{
    public class EnvironmentState : IState
    {
        private IDisposable _unsbscriber;
        private IEnvironment _environment;
        public EnvironmentState(IEnvironment environment)
        {
            _environment = environment;
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
                msg.Authentication(_environment.AuthenticationManager, (u) =>
                {
                    ((ServerConnectionContext)context).Session = _environment.CreateSession(u);
                    //context.State = new AuthenticatedState();


                    context.Connection.Subscribe((InvokeService)_environment.InvokeService);
                    _unsbscriber?.Dispose();

                }).PipeTo(msg.Id, context.Connection.Channel);
            }
        }
    }
}
