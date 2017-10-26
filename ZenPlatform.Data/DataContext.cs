using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace ZenPlatform.Data
{
    

    public class DataContext : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _activeTransaction;
        private readonly IsolationLevel _isolationLevel;
        private int _tranCount;

        public DataContext(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _isolationLevel = IsolationLevel.Snapshot;
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

        public DbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();
            if (_activeTransaction != null)
                cmd.Transaction = _activeTransaction;

            return cmd;
        }
        
        public void Dispose()
        {
            _connection.Close();
        }
    }
}
