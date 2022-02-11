using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Aquila.QueryBuilder;

namespace Aquila.Data
{
    /// <summary>
    /// Data context manager layer for manipulating and managing contexts
    /// </summary>
    public class DataContextManager : IDisposable
    {
        private SqlDatabaseType _dbType;
        private string _connectionString;
        private readonly ConcurrentDictionary<int, DataConnectionContext> _contexts;

        /// <summary>
        /// Create new context manager
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
        /// Get data context. Hides logic of creating connection from user
        /// </summary>
        /// <returns>Ready for work <see cref="DataConnectionContext"/></returns>
        public DataConnectionContext GetContext()
        {
            if (_dbType == SqlDatabaseType.Unknown)
                throw new Exception("Database is unknown");

            if (_connectionString == null)
                throw new NotSupportedException("Connection string is empty!");

            if (!_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var context))
            {
                context = new DataConnectionContext(_dbType, _connectionString, IsolationLevel.ReadCommitted);

                //NOTE: After start using AspNetCore for the backend hosting we not need hold here the contexts
                //TODO: remove this code in the future 
                //_contexts.TryAdd(Thread.CurrentThread.ManagedThreadId, context);
            }

            return context;
        }

        /// <summary>
        /// Release context from pool
        /// </summary>
        /// <param name="context"></param>
        public void ReleaseContext(DataConnectionContext context)
        {
            var key = _contexts.FirstOrDefault(x => x.Value == context).Key;
            _contexts.TryRemove(key, out context);
        }

        /// <summary>
        /// Dispose manager and all contexts created by
        /// </summary>
        public void Dispose()
        {
        }
    }
}