using System;
using System.IO;
using Aquila.Core.Instance;
using Aquila.Core.Tools;

namespace Aquila.Core.Network.States
{
    public class ProxyObjectObserver : IConnectionObserver<IConnectionContext>
    {
        private IDisposable _unsbscriber;
        private object _instanceService;
        private AqInstance _instance;
        private Guid _id;

        public ProxyObjectObserver(IConnection connection, Guid Id, object instanceService, AqInstance instance)
        {
            _instance = instance;


            _instanceService = instanceService;
            _id = Id;
            Subscribe(connection);
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestInvokeMethodProxy))
                   || type.Equals(typeof(RequestInvokeDisposeProxy))
                   || type.Equals(typeof(RequestInvokeStreamProxy));
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
                case RequestInvokeDisposeProxy _:

                    _unsbscriber?.Dispose();
                    break;
                case RequestInvokeMethodProxy methodProxy:
                    if (methodProxy.RequestId.Equals(_id))
                    {
                        // var task = _instance.InvokeService.InvokeProxy(_instanceService, methodProxy.MethodName,
                        //     methodProxy.Args);
                        //
                        // context.Connection.Channel.Send(new ResponceInvokeMethodProxy(methodProxy.Id, await task));
                    }

                    break;
                case RequestInvokeStreamProxy streamProxy:
                    if (streamProxy.RequestId.Equals(_id))
                    {
                        // var task = _instance.InvokeService.InvokeProxy(_instanceService, streamProxy.MethodName,
                        //     streamProxy.Args);
                        //
                        // var dstStream = new DataStream(streamProxy.Id, context.Connection);
                        // var srcStream = ((Stream)await task);
                        // //srcStream.Seek(0, SeekOrigin.Begin);
                        // await srcStream.CopyToAsync(dstStream);
                        //
                        // dstStream.Close();
                    }

                    break;
            }
        }
    }
}