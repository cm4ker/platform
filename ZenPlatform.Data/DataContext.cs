using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Data
{
    /// <summary>
    /// Контекст данных.
    /// Позволяет выполнять запросы в базе данных
    /// </summary>
    public class DataContext : IDisposable
    {
        private readonly DbConnection _connection;
        private DbTransaction _activeTransaction;

        private readonly IsolationLevel _isolationLevel;

        public ISqlCompiler SqlCompiller { get; }

        private int _tranCount;
        private QueryMachine _machine;

        public DataContext(SqlDatabaseType compilerType, string connectionString)
        {
            _connection = DatabaseFactory.Get(compilerType, connectionString);
            SqlCompiller = SqlCompillerBase.FormEnum(compilerType);
            _connection.Open();
            _isolationLevel = IsolationLevel.Snapshot;
            _machine = new QueryMachine();
        }

        public void BeginTransaction()
        {
            if (_activeTransaction == null)
                _activeTransaction = _connection.BeginTransaction(_isolationLevel);
            _tranCount++;
        }

        public void RollbackTransaction()
        {
            if (_tranCount > 0)
            {
                _activeTransaction.Rollback();
                _activeTransaction = null;
            }
        }

        public void CommitTransaction()
        {
            if (_tranCount > 0)
                _tranCount--;

            if (_tranCount == 0)
            {
                _activeTransaction.Commit();
                _activeTransaction = null;
            }
        }

        public DbCommand CreateCommand(Action<QueryMachine> action)
        {
            _machine.reset();
            action(_machine);
            return CreateCommand((SSyntaxNode) _machine.pop());
        }

        public DbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();
            if (_activeTransaction != null)
                cmd.Transaction = _activeTransaction;

            return cmd;
        }

        public DbCommand CreateCommand(SSyntaxNode query)
        {
            var cmd = CreateCommand();
            cmd.CommandText = SqlCompiller.Compile(query);

            return cmd;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}