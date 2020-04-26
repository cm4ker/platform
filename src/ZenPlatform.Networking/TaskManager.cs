using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Network
{
    public class TaskManager : ITaskManager
    {
        private readonly List<InvokeContext> _invokes;
        private readonly ILogger<TaskManager> _logger;

        public TaskManager(ILogger<TaskManager> logger)
        {
            _logger = logger;
            _invokes = new List<InvokeContext>();
        }

        private void StartTask(InvokeContext invokeContext)
        {
            if (invokeContext == null)
                throw new NullReferenceException("THIS IS SUPER MESSAGE");

            _logger.Trace("Start task id: {0}.", invokeContext.Task.Id);
            _invokes.Add(invokeContext);

            invokeContext.Task.ContinueWith(t => FinishTask(invokeContext));
        }

        private void FinishTask(InvokeContext invokeContext)
        {
            _logger.Trace("End task id: {0}.", invokeContext.Task.Id);
            _invokes.Remove(invokeContext);
        }

        public Task<object> RunTask(ISession session, Func<InvokeContext, object> action)
        {
            var canceller = new CancellationTokenSource();
            Task<object> task = null;
            InvokeContext context = null;
            
            task = new Task<object>(
                (o) =>
                {
                    context = new InvokeContext(task, canceller, session);

                    StartTask(context);
                    using (canceller.Token.Register(Thread.CurrentThread.Interrupt))
                    {
                        try
                        {
                            return action(context);
                        }
                        finally
                        {
                            // _taskManager.FinishTask(invokeContext);
                        }
                    }
                }, null, canceller.Token, TaskCreationOptions.LongRunning);
            
            task.Start(TaskScheduler.Current);
         
            return task;
        }
    }
}