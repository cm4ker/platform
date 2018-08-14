using System;
using System.Collections.Generic;
using System.Threading;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Data
{
    /// <summary>
    /// Менеджер контекста данных
    /// Позволяет получить доступ к данным
    /// Клиент может получить доступ к контексту
    /// </summary>
    public class DataContextManger
    {
        private readonly SqlDatabaseType _dbType;
        private readonly string _connectionString;
        private readonly Dictionary<int, DataContext> _contexts;

        /// <summary>
        /// Создать новый менеджер контекстов
        /// </summary>
        /// <param name="connectionString"></param>
        public DataContextManger(SqlDatabaseType dbType, string connectionString)
        {
            _dbType = dbType;
            _connectionString = connectionString;
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
                context = new DataContext(_dbType, _connectionString);
                _contexts.Add(Thread.CurrentThread.ManagedThreadId, context);
            }

            return context;
        }
    }
}