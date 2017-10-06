using System.Data;
using System.Data.SqlClient;

namespace QueryCompiler.Schema
{
    public interface IDBContext
    {
        SqlConnection Connection { get; }
        void BeginTransaction(IsolationLevel isolation);
        void CommitTransaction();
        SqlCommand CreateCommand();
        SqlCommand CreateCommand(string sql);
        void Dispose();
        void ExecNonQueryBatch(DBBatch batch);
        void RollbackTransaction();
    }
}