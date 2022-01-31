using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aquila.Core.Contracts.Network
{
    /// <summary>
    /// Контекст выполнения метода
    /// </summary>
    public class InvokeContext
    {
        /// <summary>
        /// Задача
        /// </summary>
        public Task Task { get; private set; }

        /// <summary>
        /// Токен отмены задачи
        /// </summary>
        public CancellationTokenSource CancellationToken { get; private set; }

        /// <summary>
        /// Текущая сессия
        /// </summary>
        public ISession Session { get; private set; }

        public InvokeContext(Task task, CancellationTokenSource cancellationToken, ISession session)
        {
            Task = task;
            CancellationToken = cancellationToken;
            Session = session;
        }
    }

    /// <summary>
    /// Интерфейс для вызова удаленных процедур
    /// </summary>
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

        object GetRequiredService(Type type);

        Task<object> InvokeProxy(ISession session, object instanceObject, string methodName, object[] args);
    }
}