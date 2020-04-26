using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Network;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;

namespace ZenPlatform.Networking
{
    public class InvokeService : IInvokeService //, IConnectionObserver<IConnectionContext>
    {
        private readonly Dictionary<Route, ParametricMethod> methods = new Dictionary<Route, ParametricMethod>();
        private readonly Dictionary<Route, StreamMethod> streamMethods = new Dictionary<Route, StreamMethod>();
        private readonly Dictionary<Guid, object> services = new Dictionary<Guid, object>();
        private readonly ILogger _logger;
        private readonly ITaskManager _taskManager;
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<IConnection, IDisposable> _unsbscribers;

        public InvokeService(ILogger<InvokeService> loggery, ITaskManager taskManager, IServiceProvider serviceProvider)
        {
            _logger = loggery;
            _taskManager = taskManager;
            _serviceProvider = serviceProvider;
            _unsbscribers = new Dictionary<IConnection, IDisposable>();
        }


        public async Task<object> Invoke(Route route, ISession session, params object[] arg)
        {
            if (!methods.ContainsKey(route))
                throw new InvokeException($"Method not found, route = {route.ToString()}");

            var task = _taskManager.RunTask(session, ic =>
            {
                object result = null;

                var c = ExecutionContext.Capture();
                ExecutionContext.Run(c, state =>
                {
                    ContextHelper.SetContext(new PlatformContext(ic.Session));
                    result = methods[route](ic, arg);
                }, null);

                return result;
            });

            return await task;
        }


        public void Register(Route route, ParametricMethod method)
        {
            if (!methods.ContainsKey(route))
            {
                _logger.Trace("Add method '{0}'", route.ToString());
                methods.Add(route, method);
            }
            else
            {
                _logger.Debug("Method '{0}' already exist", route.ToString());
            }
        }

        public Task InvokeStream(Route route, ISession session, Stream stream, params object[] arg)
        {
            if (!streamMethods.ContainsKey(route))
                throw new InvokeException($"Method not found, route = {route.ToString()}");


            var task = _taskManager.RunTask(session, ic =>
            {
                var c = ExecutionContext.Capture();
                ExecutionContext.Run(c, state =>
                {
                    ContextHelper.SetContext(new PlatformContext(ic.Session));
                    streamMethods[route](ic, stream, arg);
                }, null);

                return null;
            });

            return task;
        }

        public Task<object> InvokeProxy(ISession session, object instanceObject, string methodName, object[] args)
        {
            return _taskManager.RunTask(session, ic =>
            {
                MethodInfo methodInfo = instanceObject.GetType().GetMethod(methodName);

                return methodInfo.Invoke(instanceObject, args);
            });
        }

        public void RegisterStream(Route route, StreamMethod method)
        {
            if (!streamMethods.ContainsKey(route))
            {
                _logger.Trace("Add method '{0}'", route.ToString());
                streamMethods.Add(route, method);
            }
            else
            {
                _logger.Debug("Method '{0}' already exist", route.ToString());
            }
        }

        public object GetRequiredService(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }

        /*
        public void Subscribe(IConnection connection)
        {
            _unsbscribers.Add(connection, connection.Subscribe(this));
        }

        public bool CanObserve(Type type)
        {
            return false;
                //type.Equals(typeof(RequestInvokeUnaryNetworkMessage)) || type.Equals(typeof(StartInvokeStreamNetworkMessage));
               //  || type.Equals(typeof(RequestInvokeInstanceProxy)) || type.Equals(typeof(RequestInvokeMethodProxy)) 
               //  || type.Equals(typeof(RequestInvokeDisposeProxy));
                
        }

        public void OnCompleted(IConnectionContext context)
        {
            if (_unsbscribers.ContainsKey(context.Connection))
            {
                _unsbscribers[context.Connection].Dispose();
                _unsbscribers.Remove(context.Connection);
            }
        }

        public void OnError(IConnectionContext context, Exception error)
        {
            if (_unsbscribers.ContainsKey(context.Connection))
            {
                _unsbscribers[context.Connection].Dispose();
                _unsbscribers.Remove(context.Connection);
            }
        }

        

        public async  void OnNext(IConnectionContext context, INetworkMessage value)
        {
            switch (value)
            {
                case RequestInvokeInstanceProxy instanceProxy:
                    var type = Type.GetType(instanceProxy.InterfaceName);
                    var service = _serviceProvider.GetRequiredService(type);
                    services.Add(instanceProxy.Id, service);
                    break;
                case RequestInvokeDisposeProxy disposeProxy:
                    services.Remove(disposeProxy.RequestId);
                    break;
                case RequestInvokeMethodProxy methodProxy:
                    if (services.ContainsKey(methodProxy.RequestId))
                    {

                        var task = _taskManager.RunTask(((ServerConnectionContext)context).Session, ic =>
                        {

                            var invokeService = services[methodProxy.RequestId];

                            MethodInfo methodInfo = invokeService.GetType().GetMethod(methodProxy.MethodName);

                            return methodInfo.Invoke(invokeService, methodProxy.Args);
                        });
        
                        context.Connection.Channel.Send(new ResponceInvokeMethodProxy(methodProxy.Id, await task));
                    }
                    break;
                case RequestInvokeUnaryNetworkMessage invoke:
                    if (context is ServerConnectionContext serverContext)
                    {
                        var res = Invoke(invoke.Route, serverContext.Session, invoke.Args);
                        //.PipeTo(invoke.Id, serverContext.Connection.Channel);

                        var responce = new ResponceInvokeUnaryNetworkMessage(invoke.Id, await res);

                        serverContext.Connection.Channel.Send(responce);
                    }
                    break;
                case StartInvokeStreamNetworkMessage invokeStream:
                    if (context is ServerConnectionContext serverContext2)
                    {
                        var stream = new DataStream(invokeStream.Id, context.Connection);
                        await InvokeStream(invokeStream.Route, serverContext2.Session, stream, invokeStream.Request);
                    }
                    break;
            }
        }
        */
    }
}