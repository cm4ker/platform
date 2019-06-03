using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Sessions;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network
{
    public delegate object ParametricMethod(InvokeContext context, params object[] list);
    public delegate void StreamMethod(InvokeContext context, Stream stream, params object[] list);
    
    public class InvokeContext
    {
        public Task Task { get; private set; }
        public CancellationTokenSource Canceller { get; private set; }
        public ISession Session { get; private set; }

        public InvokeContext(Task task, CancellationTokenSource canceller, ISession session)
        {
            Task = task;
            Canceller = canceller;
            Session = session;
        }
    }
    
    public interface IInvokeService
    {
        /// <summary>
        /// Добавляет метод на сервер удаленных процедур
        /// </summary>
        /// <param name="route">Маршрут метода</param>
        /// <param name="method">Метод</param>
        void Register(Route route, ParametricMethod method);
        
        /// <summary>
        /// Вызывает метод зарегистрированный на сервере
        /// </summary>
        /// <param name="route">Маршрут метода</param>
        /// <param name="arg">Параметры метода</param>
        /// <returns></returns>
        Task<object> Invoke(Route route, ISession session, params object[] arg);

        void RegisterStream(Route route, StreamMethod method);

        Task InvokeStream(Route route, ISession session, Stream stream, params object[] arg);


    }
}
