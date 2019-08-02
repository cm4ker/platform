using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network.States
{
    public class InvokeObserver: IConnectionObserver<IConnectionContext>
    {
        private IEnvironment _environment;
        private IDisposable _unsbscriber;
        public InvokeObserver(IEnvironment environment)
        {
            _environment = environment;
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestInvokeInstanceProxy))
                || type.Equals(typeof(RequestInvokeUnaryNetworkMessage)) || type.Equals(typeof(StartInvokeStreamNetworkMessage));

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

        public async void OnNext(IConnectionContext context, INetworkMessage value)
        {
            switch (value)
            {
                case RequestInvokeInstanceProxy instance:
                    new ProxyObjectObserver(context.Connection, instance, _environment);
                    break;
                case RequestInvokeUnaryNetworkMessage invoke:
                    if (context is ServerConnectionContext serverContext)
                    {
                        var res = _environment.InvokeService.Invoke(invoke.Route, serverContext.Session, invoke.Args);

                        var responce = new ResponceInvokeUnaryNetworkMessage(invoke.Id, await res);

                        serverContext.Connection.Channel.Send(responce);
                    }
                    break;
                case StartInvokeStreamNetworkMessage invokeStream:
                    if (context is ServerConnectionContext serverContext2)
                    {
                        var stream = new DataStream(invokeStream.Id, context.Connection);
                        await _environment.InvokeService.InvokeStream(invokeStream.Route, serverContext2.Session, stream, invokeStream.Request);
                    }
                    break;
            }
        }
    }
}
