using System;
using System.Collections.Generic;
using System.Threading;

namespace ZenPlatform.Data
{
    /// <summary>
    /// Менеджер контекста данных
    /// Позволяет получить доступ к данным
    /// Клиент может получить доступ к контексту
    /// </summary>
    public class DataContextManger
    {
        private Dictionary<int, DataContext> _contexts;

        public DataContextManger()
        {
            _contexts = new Dictionary<int, DataContext>();
        }

        /// <summary>
        /// Получить контекст данных.
        /// Внимание, на каждый поток выдаётся отдельный контекст данных.
        /// Так что транзакция обязана выполниться в одном потоке.
        /// </summary>
        /// <returns></returns>
        public DataContext GetContext()
        {
            if (!_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var context))
            {
                //TODO: Брать connection string и файла конфигурации
                context = new DataContext("Data source=(local);Initial catalog=TestDatabase; Integrated security=true;");

                _contexts.Add(Thread.CurrentThread.ManagedThreadId, context);
            }

            return context;
        }
    }
}