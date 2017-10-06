using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace QueryCompiler.Schema
{
    public abstract class DBContext : IDBContext, IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _activeTransaction;
        private int _tranCount;

        protected DBContext()
        {
            _tranCount = 0;
            // _transactions = new Stack<SqlTransaction>();
        }

        protected DBContext(SqlConnection conn) : this()
        {
            _connection = conn ?? throw new NullReferenceException();
        }

        protected DBContext(string connectionString, bool isUDLFile) : this()
        {
            if (isUDLFile)
            {
                _connection = GetSqlConnection("WSkladUdl.udl");
            }
            else
            {
                _connection = CreateSqlConnection(connectionString);
            }

            if (_connection is null) throw new NullReferenceException();
        }

        public void BeginTransaction(IsolationLevel isolation)
        {
            if (_activeTransaction is null)
                _activeTransaction = _connection.BeginTransaction(isolation);
            _tranCount++;
        }

        public void CommitTransaction()
        {
            if (_tranCount == 1)
            {
                _activeTransaction.Commit();
                _activeTransaction = null;
            }
            _tranCount--;
        }
        public void RollbackTransaction()
        {
            if (_tranCount > 0)
            {
                _activeTransaction.Rollback();
                _activeTransaction = null;
                _tranCount = 0;
            }
        }

        public SqlCommand CreateCommand(string sql)
        {
            if (_connection is null)
            {
                throw new NullReferenceException();
            }

            var cmd = _connection.CreateCommand();
            if (_activeTransaction != null)
                cmd.Transaction = _activeTransaction;
            cmd.CommandTimeout = 0;
            cmd.CommandText = sql;

            return cmd;
        }
        public SqlCommand CreateCommand()
        {
            if (_connection is null)
            {
                throw new NullReferenceException();
            }

            var cmd = _connection.CreateCommand();
            if (_activeTransaction != null)
                cmd.Transaction = _activeTransaction;

            return cmd;
        }

        public SqlConnection Connection
        {
            get { return _connection; }
        }

        public void ExecNonQueryBatch(DBBatch batch)
        {
            if (batch.Parameters.Count > 2000)
            {
                try
                {
                    BeginTransaction(IsolationLevel.Snapshot);
                    using (var cmd = CreateCommand())
                    {
                        foreach (var query in batch.Queryes)
                        {
                            if (query is IDataChangeQuery)
                                foreach (var param in (query as IDataChangeQuery).Parameters)
                                {
                                    cmd.Parameters.Add(param.SqlParameter);
                                }

                            cmd.CommandText = query.Compile();
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }
                    CommitTransaction();
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                }
            }
            else
            {
                using (var cmd = CreateCommand(batch.Compile()))
                {
                    foreach (var parameter in batch.Parameters)
                    {
                        cmd.Parameters.Add(parameter.SqlParameter);
                    }
                    var fetched = cmd.ExecuteNonQuery();
                }
            }
        }

        private string GetSqlConnectionStringFromUDL(string path)
        {
            using (var fs = new StreamReader(path))
            {
                var connectionString = fs.ReadToEnd();
                var p = connectionString.Split(';');
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                scsb.InitialCatalog = p.FirstOrDefault(x => x.ToLower().Contains("initial catalog")).Split('=')[1];
                scsb.DataSource = p.FirstOrDefault(x => x.ToLower().Contains("data source")).Split('=')[1];
                scsb.Password = p.FirstOrDefault(x => x.ToLower().Contains("password")).Split('=')[1];
                scsb.UserID = p.FirstOrDefault(x => x.ToLower().Contains("user id")).Split('=')[1];

                return scsb.ConnectionString;
            }
        }
        private SqlConnection CreateSqlConnection(string connectionString)
        {
            try
            {

                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch
            {
                return null;
            }

        }
        private SqlConnection GetSqlConnection(string filename)
        {
            var connString = GetSqlConnectionStringFromUDL(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
            return CreateSqlConnection(connString);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}