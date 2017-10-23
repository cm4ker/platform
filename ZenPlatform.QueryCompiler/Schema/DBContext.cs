using System;
using System.Data;
using System.IO;
using System.Linq;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.QueryCompiler.Schema
{
    //public abstract class DBContext : IDBContext, IDisposable
    //{
    //    private readonly SqlConnection _connection;
    //    private SqlTransaction _activeTransaction;
    //    private int _tranCount;

    //    protected DBContext()
    //    {
    //        _tranCount = 0;
    //        // _transactions = new Stack<SqlTransaction>();
    //    }

    //    protected DBContext(SqlConnection conn) : this()
    //    {
    //        _connection = conn ?? throw new NullReferenceException();
    //    }

    //    protected DBContext(string connectionString, bool isUDLFile) : this()
    //    {
    //        if (isUDLFile)
    //        {
    //            _connection = GetSqlConnection("WSkladUdl.udl");
    //        }
    //        else
    //        {
    //            _connection = CreateSqlConnection(connectionString);
    //        }

    //        if (_connection is null) throw new NullReferenceException();
    //    }

    //    public void BeginTransaction(IsolationLevel isolation)
    //    {
    //        if (_activeTransaction is null)
    //            _activeTransaction = _connection.BeginTransaction(isolation);
    //        _tranCount++;
    //    }

    //    public void CommitTransaction()
    //    {
    //        if (_tranCount == 1)
    //        {
    //            _activeTransaction.Commit();
    //            _activeTransaction = null;
    //        }
    //        _tranCount--;
    //    }
    //    public void RollbackTransaction()
    //    {
    //        if (_tranCount > 0)
    //        {
    //            _activeTransaction.Rollback();
    //            _activeTransaction = null;
    //            _tranCount = 0;
    //        }
    //    }

    //    public SqlCommand CreateCommand(string sql)
    //    {
    //        if (_connection is null)
    //        {
    //            throw new NullReferenceException();
    //        }

    //        var cmd = _connection.CreateCommand();
    //        if (_activeTransaction != null)
    //            cmd.Transaction = _activeTransaction;
    //        cmd.CommandTimeout = 0;
    //        cmd.CommandText = sql;

    //        return cmd;
    //    }
    //    public SqlCommand CreateCommand()
    //    {
    //        if (_connection is null)
    //        {
    //            throw new NullReferenceException();
    //        }

    //        var cmd = _connection.CreateCommand();
    //        if (_activeTransaction != null)
    //            cmd.Transaction = _activeTransaction;

    //        return cmd;
    //    }

    //    public SqlConnection Connection
    //    {
    //        get { return _connection; }
    //    }

    //    public void ExecNonQueryBatch(DBBatch batch)
    //    {
    //        if (batch.Parameters.Count > 2000)
    //        {
    //            try
    //            {
    //                BeginTransaction(IsolationLevel.Snapshot);

    //                var microBatch = new DBBatch();
    //                var currentParams = 0;
    //                foreach (var query in batch.Queryes)
    //                {
    //                    if (query is IDataChangeQuery)
    //                    {
    //                        var dcQuery = query as IDataChangeQuery;

    //                        if (currentParams + dcQuery.Parameters.Count <= 2000)
    //                        {
    //                            microBatch.AddQuery(query);
    //                            currentParams += dcQuery.Parameters.Count;
    //                        }
    //                        else
    //                        {
    //                            ExecNonQueryBatch(microBatch);
    //                            microBatch.Queryes.Clear();
    //                            currentParams = 0;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        microBatch.AddQuery(query);
    //                    }
    //                }

    //                if (currentParams > 2000) throw new Exception("Parameters count overflow");

    //                if (microBatch.Queryes.Count > 0)
    //                    ExecNonQueryBatch(microBatch);

    //                CommitTransaction();
    //            }
    //            catch (Exception e)
    //            {
    //                RollbackTransaction();
    //                throw e;
    //            }
    //        }
    //        else
    //        {
    //            using (var cmd = CreateCommand(batch.Compile()))
    //            {
    //                foreach (var parameter in batch.Parameters)
    //                {
    //                    cmd.Parameters.Add(parameter.SqlParameter);
    //                }
    //                var fetched = cmd.ExecuteNonQuery();
    //            }
    //        }
    //    }

    //    private string GetSqlConnectionStringFromUDL(string path)
    //    {
    //        using (var fs = new StreamReader(path))
    //        {
    //            var connectionString = fs.ReadToEnd();
    //            var p = connectionString.Split(';');

    //            var initialCatalog = p.FirstOrDefault(x => x.ToLower().Contains("initial catalog")) ??
    //                                 throw new Exception("need requered parameter Initial Catalog");
    //            var dataSource = p.FirstOrDefault(x => x.ToLower().Contains("data source")) ??
    //                             throw new Exception("need requered  parameter Data Source");
    //            var password = p.FirstOrDefault(x => x.ToLower().Contains("password")) ?? "password=";
    //            var userId = p.FirstOrDefault(x => x.ToLower().Contains("user id")) ?? "user id=";
    //            var integratedSecurity = p.FirstOrDefault(x => x.ToLower().Contains("integrated security"));

    //            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
    //            scsb.InitialCatalog = initialCatalog.Split('=')[1];
    //            scsb.DataSource = dataSource.Split('=')[1];

    //            if (integratedSecurity is null)
    //            {
    //                scsb.Password = password.Split('=')[1];
    //                scsb.UserID = userId.Split('=')[1];
    //            }
    //            else
    //            {
    //                scsb.IntegratedSecurity = true;
    //            }

    //            scsb.PersistSecurityInfo = true;

    //            return scsb.ConnectionString;
    //        }
    //    }
    //    private SqlConnection CreateSqlConnection(string connectionString)
    //    {
    //        try
    //        {
    //            var connection = new SqlConnection(connectionString);
    //            connection.Open();

    //            return connection;
    //        }
    //        catch (SqlException e) when (e.Number == 4060)
    //        {

    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //            throw;
    //        }

    //    }
    //    private SqlConnection GetSqlConnection(string filename)
    //    {
    //        var connString = GetSqlConnectionStringFromUDL(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
    //        return CreateSqlConnection(connString);
    //    }

    //    public void Dispose()
    //    {
    //        _connection.Close();
    //        _connection.Dispose();
    //    }
    //}
}