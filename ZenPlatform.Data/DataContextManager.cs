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
    public class DataContextManager : IDataContextManager
    {
        private SqlDatabaseType? _dbType;
        private string _connectionString;
        private readonly Dictionary<int, DataContext> _contexts;

        /// <summary>
        /// Создать новый менеджер контекстов
        /// </summary>
        /// <param name="connectionString"></param>
        public DataContextManager()
        {
            _contexts = new Dictionary<int, DataContext>();
 
        }

        public void Initialize(SqlDatabaseType dbType, string connectionString)
        {
            _dbType = dbType;
            
            _connectionString = connectionString;
            SqlCompiler = SqlCompillerBase.FormEnum(dbType);
        }

        public ISqlCompiler SqlCompiler { get; private set; }

        /// <summary>
        /// Получить контекст данных.
        /// Внимание, на каждый поток выдаётся отдельный контекст данных.
        /// Так что транзакция обязана выполниться в одном потоке.
        /// </summary>
        /// <returns></returns>
        public DataContext GetContext()
        {
            if (_dbType == null || _connectionString == null) throw new NotSupportedException("DataContextManager not initialized!");
            if (!_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var context))
            {
                context = new DataContext(_dbType.Value, _connectionString);
                _contexts.Add(Thread.CurrentThread.ManagedThreadId, context);
            }

            return context;
        }
    }
}