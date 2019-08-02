using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network.States
{
    public class ProxyObjectObserver : IConnectionObserver<IConnectionContext>
    {
        private IDisposable _unsbscriber;
        private object _instanceService;
        private IEnvironment _environment;
        private Guid _id;

        public ProxyObjectObserver(IConnection connection, RequestInvokeInstanceProxy instanceProxy, IEnvironment environment)
        {
            _environment = environment;
            var type = Type.GetType(instanceProxy.InterfaceName);
            _instanceService = _environment.InvokeService.GetRequiredService(type);
            _id = instanceProxy.Id;
            Subscribe(connection);



        }
        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestInvokeMethodProxy))

                || type.Equals(typeof(RequestInvokeDisposeProxy));

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
                case RequestInvokeDisposeProxy disposeProxy:
                    _unsbscriber?.Dispose();
                    break;
                case RequestInvokeMethodProxy methodProxy:
                    if (methodProxy.RequestId.Equals(_id))
                    {
                        var task = _environment.InvokeService.InvokeProxy(((ServerConnectionContext)context).Session,
                            _instanceService, methodProxy.MethodName, methodProxy.Args);

                        context.Connection.Channel.Send(new ResponceInvokeMethodProxy(methodProxy.Id, await task));
                    }
                    break;
            }
            }
    }
}
