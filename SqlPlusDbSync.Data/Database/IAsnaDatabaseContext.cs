using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QueryCompiler;

namespace SqlPlusDbSync.Data.Database
{
    public interface IAsnaDatabaseContext
    {
        SqlConnection Connection { get; }
        bool IsServer { get; }
        List<Guid?> PointId { get; }

        void BeginTransaction(IsolationLevel isolation);
        void CommitTransaction();
        SqlCommand CreateCommand();
        SqlCommand CreateCommand(string sql);
        void Dispose();
        void ExecNonQueryBatch(DBBatch batch);
        byte[] GetSyncVersion(Guid pointId);
        byte[] GetVersion();
        void RollbackTransaction();
    }
}