using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network.States
{
    public class AuthenticationObserver : IConnectionObserver<IConnectionContext>
    {
        private IDisposable _unsbscriber;
        private IEnvironment _environment;

        public AuthenticationObserver(IEnvironment environment)
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
                    ((ServerConnectionContext) context).Session = _environment.CreateSession(u);
                    //context.State = new AuthenticatedState();


                    //context.Connection.Subscribe((InvokeService)_environment.InvokeService);
                    var state = new InvokeObserver(_environment);
                    state.Subscribe(context.Connection);
                    _unsbscriber?.Dispose();
                }).PipeTo(msg.Id, context.Connection.Channel);
            }
        }
    }
}