using System;
using Aquila.Core.Instance;
using Aquila.Core.Tools;
using Aquila.Serializer;

namespace Aquila.Core.Network.States
{
    public class InvokeObserver : IConnectionObserver<IConnectionContext>
    {
        private AqInstance _instance;
        private IDisposable _unsbscriber;

        public InvokeObserver(AqInstance instance)
        {
            _instance = instance;
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(RequestInvokeInstanceProxy))
                   || type.Equals(typeof(RequestInvokeUnaryNetworkMessage))
                   || type.Equals(typeof(RequestInvokeUnaryByteArgsNetworkMessage))
                   || type.Equals(typeof(StartInvokeStreamNetworkMessage));
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
                    // var type = Type.GetType(instance.InterfaceName);
                    // var instanceService = _instance.InvokeService.GetRequiredService(type);
                    //
                    // new ProxyObjectObserver(context.Connection, instance.Id, instanceService, _instance);
                    break;
                case RequestInvokeUnaryNetworkMessage invoke:
                    if (context is ServerConnectionContext serverContext)
                    {
                        // var res = _instance.InvokeService.Invoke(invoke.Route, invoke.Args);
                        //
                        // var responce = new ResponceInvokeUnaryNetworkMessage(invoke.Id, await res);
                        //
                        // serverContext.Connection.Channel.Send(responce);
                    }

                    break;

                case RequestInvokeUnaryByteArgsNetworkMessage invoke:
                    if (context is ServerConnectionContext srvContext)
                    {
                        // PlatformSerializer serializer = new PlatformSerializer();
                        // var args = serializer.Deserialize(invoke.Args, false);
                        // var res = _instance.InvokeService.Invoke(invoke.Route, args);
                        //
                        //
                        // var result = serializer.Serialize(await res);
                        // var responce = new ResponceInvokeUnaryByteArgsNetworkMessage(invoke.Id, result);
                        //
                        // srvContext.Connection.Channel.Send(responce);
                    }

                    break;


                case StartInvokeStreamNetworkMessage invokeStream:
                    if (context is ServerConnectionContext serverContext2)
                    {
                        // var stream = new DataStream(invokeStream.Id, context.Connection);
                        // await _instance.InvokeService.InvokeStream(invokeStream.Route, 
                        //     stream, invokeStream.Request);
                    }

                    break;
            }
        }
    }
}