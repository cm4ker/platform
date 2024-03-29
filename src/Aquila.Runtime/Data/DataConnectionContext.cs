﻿using System;
using System.Data;
using System.Data.Common;
using Aquila.QueryBuilder;
using Aquila.QueryBuilder.Model;

namespace Aquila.Data
{
    /// <summary>
    /// Context of the connection to the database
    /// </summary>
    public class DataConnectionContext : IDisposable
    {
        private readonly DbConnection _connection;
        private DbTransaction _activeTransaction;

        private readonly IsolationLevel _isolationLevel;

        public SqlCompiler SqlCompiller { get; }


        private int _tranCount;
        private QueryMachine _machine;

        public DataConnectionContext(SqlDatabaseType compilerType, string connectionString,
            IsolationLevel isolationLevel = IsolationLevel.Snapshot)
        {
            _connection = DatabaseFactory.Get(compilerType, connectionString);
            SqlCompiller = QueryBuilder.SqlCompiler.FormEnum(compilerType);
            Types = new SqlServerDbTypesContract();
            _connection.Open();
            _isolationLevel = isolationLevel;
            _machine = new QueryMachine();
        }

        public IDbTypesContract Types { get; }

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
            return CreateCommand((SSyntaxNode)_machine.pop());
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
            _connection.Dispose();
        }
    }
}