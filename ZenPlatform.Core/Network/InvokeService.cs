using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Sessions;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network
{
    public class InvokeService : IInvokeService
    {
        private readonly Dictionary<Route, ParametricMethod> methods = new Dictionary<Route, ParametricMethod>();
        private readonly Dictionary<Route, StreamMethod> streamMethods = new Dictionary<Route, StreamMethod>();
        private readonly ILogger _logger;
        private readonly ITaskManager _taskManager;

        public InvokeService(ILogger<InvokeService> loggery, ITaskManager taskManager)
        {
            _logger = loggery;
            _taskManager = taskManager;
        }


        public async Task<object> Invoke(Route route, ISession session, params object[] arg)
        {

            if (!methods.ContainsKey(route))
                throw new InvokeException($"Method not found, route = {route.ToString()}");

            var canceller = new CancellationTokenSource();

            Task<object> task = null;

            task = Task.Factory.StartNew(() =>
            {

                using (canceller.Token.Register(Thread.CurrentThread.Interrupt))
                {



                    var invokeContext = new InvokeContext(task, canceller, session);
                    try
                    {
                        _taskManager.StartTask(invokeContext);
                        var result = methods[route](invokeContext, arg);
                        return result;
                    }

                    finally
                    {
                        _taskManager.FinishTask(invokeContext);
                    }

                }
            }, canceller.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

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

            var canceller = new CancellationTokenSource();

            Task task = null;

            task = Task.Factory.StartNew(() =>
            {

                using (canceller.Token.Register(Thread.CurrentThread.Interrupt))
                {



                    var invokeContext = new InvokeContext(task, canceller, session);
                    try
                    {
                        _taskManager.StartTask(invokeContext);
                        streamMethods[route](invokeContext, stream, arg);

                    }

                    finally
                    {
                        _taskManager.FinishTask(invokeContext);
                    }

                }
            }, canceller.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            return task;
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
    }

}
