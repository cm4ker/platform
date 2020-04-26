using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;
using ZenPlatform.Serializer;

namespace ZenPlatform.Core.Network.States
{
    public class InvokeObserver : IConnectionObserver<IConnectionContext>
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
                    var type = Type.GetType(instance.InterfaceName);
                    var instanceService = _environment.InvokeService.GetRequiredService(type);

                    new ProxyObjectObserver(context.Connection, instance.Id, instanceService, _environment);
                    break;
                case RequestInvokeUnaryNetworkMessage invoke:
                    if (context is ServerConnectionContext serverContext)
                    {
                        var res = _environment.InvokeService.Invoke(invoke.Route, serverContext.Session, invoke.Args);

                        var responce = new ResponceInvokeUnaryNetworkMessage(invoke.Id, await res);

                        serverContext.Connection.Channel.Send(responce);
                    }

                    break;

                case RequestInvokeUnaryByteArgsNetworkMessage invoke:
                    if (context is ServerConnectionContext srvContext)
                    {
                        PlatformSerializer serializer = new PlatformSerializer();
                        var args = serializer.Deserialize(invoke.Args, false);
                        var res = _environment.InvokeService.Invoke(invoke.Route, srvContext.Session, args);


                        var result = serializer.Serialize(await res);
                        var responce = new ResponceInvokeUnaryByteArgsNetworkMessage(invoke.Id, result);

                        srvContext.Connection.Channel.Send(responce);
                    }

                    break;


                case StartInvokeStreamNetworkMessage invokeStream:
                    if (context is ServerConnectionContext serverContext2)
                    {
                        var stream = new DataStream(invokeStream.Id, context.Connection);
                        await _environment.InvokeService.InvokeStream(invokeStream.Route, serverContext2.Session,
                            stream, invokeStream.Request);
                    }

                    break;
            }
        }
    }
}