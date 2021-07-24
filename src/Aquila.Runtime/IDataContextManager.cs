using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Aquila.QueryBuilder;

namespace Aquila.Data
{
    /// <summary>
    /// Менеджер контекста данных
    /// Позволяет получить доступ к данным
    /// Клиент может получить доступ к контексту
    /// </summary>
    public class DataContextManager
    {
        private SqlDatabaseType _dbType;
        private string _connectionString;
        private readonly ConcurrentDictionary<int, DataConnectionContext> _contexts;

        /// <summary>
        /// Создать новый менеджер контекстов
        /// </summary>
        /// <param name="connectionString"></param>
        public DataContextManager()
        {
            _contexts = new ConcurrentDictionary<int, DataConnectionContext>();
        }

        public SqlDatabaseType DatabaseType
        {
            get => _dbType;
        }

        public void Initialize(SqlDatabaseType dbType, string connectionString)
        {
            _dbType = dbType;

            _connectionString = connectionString;
            SqlCompiler = SqlCompiler.FormEnum(dbType);
        }

        public SqlCompiler SqlCompiler { get; private set; }

        /// <summary>
        /// Получить контекст данных.
        /// Внимание, на каждый поток выдаётся отдельный контекст данных.
        /// Так что транзакция обязана выполниться в одном потоке.
        /// </summary>
        /// <returns></returns>
        public DataConnectionContext GetContext()
        {
            if (_dbType == SqlDatabaseType.Unknown)
                throw new Exception("Database is unknown");

            if (_connectionString == null)
                throw new NotSupportedException("Connection string is empty!");

            if (!_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var context))
            {
                context = new DataConnectionContext(_dbType, _connectionString, IsolationLevel.ReadCommitted);
                _contexts.TryAdd(Thread.CurrentThread.ManagedThreadId, context);
            }

            return context;
        }
    }
}