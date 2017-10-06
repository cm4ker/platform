using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using QueryCompiler.Schema;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Data.Database
{
    public class AsnaDatabaseContext : DBContext
    {
        private bool? _isServer;
        private Guid? _generalPoint;
        private List<Guid> _locatedPoints;

        

        public AsnaDatabaseContext() : base("WSkladUdl.udl", true)
        {

        }

        public AsnaDatabaseContext(string connectionString) : base(connectionString, false)
        {


        }

        public bool IsServer
        {
            get
            {
                if (_isServer is null)
                    using (var cmd = CreateCommand("SELECT SC_Msg From utb_Replication_SrvConst"))
                    {
                        _isServer = !Convert.ToBoolean(cmd.ExecuteScalar());

                    }
                return _isServer.Value;

            }
        }

        public Guid GeneralPoint
        {
            get
            {
                if (_generalPoint is null)
                    using (var cmd = CreateCommand("SELECT TableRowGuid FROM CLL where CLL_TypeCode = 0"))
                    {
                        try
                        {
                            _generalPoint = (Guid)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            return Guid.Empty;
                        }
                    }

                return _generalPoint.Value;
            }
        }

        public List<Guid> LocatedPoints
        {
            get
            {
                if (_locatedPoints is null)
                {
                    _locatedPoints = new List<Guid>();
                    using (var cmd = CreateCommand(@"
                                            SELECT distinct
	                                            Cll.TableRowGUID
                                            FROM  dbo.Kls_Const KC with(nolock)
                                            INNER JOIN dbo.KlsCmpLnk KL with(nolock) ON (KL.Kls_UniCode = KC.Kls_UniCode)
                                            INNER JOIN dbo.Cll  with(nolock) ON (Cll.Cll_UniCode = KL.Cmp_UniCode) 
                                            WHERE KC.KC_Type = 1 and KC.KC_Value = 5
"))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _locatedPoints.Add(reader.GetGuid(0));
                            }
                        }

                    }
                }
                return _locatedPoints;
            }
        }

        public string GetNameFromGuid(Guid id)
        {
            if (id == Guid.Empty) return "Unknown";
            using (var cmd = CreateCommand("SELECT CLL_ShortName FROM CLL WHERE TableRowGuid = @pointId"))
            {
                cmd.Parameters.Add(new SqlParameter("@pointId", id));
                var name = cmd.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(name))
                    return "Unknown";
                return name;
            }
        }

        public byte[] GetVersion()
        {
            using (var cmd = CreateCommand("SELECT MAX(Version) FROM __Versions"))
            {
                var result = cmd.ExecuteScalar();
                if (result is null) result = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, };
                return (byte[])result;
            }


        }

        public byte[] GetSyncVersion(Guid pointId)
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT SyncronizedVersion FROM __Syncronization WHERE PointId = @pointid";
                cmd.Parameters.Add(new SqlParameter("@pointid", pointId));
                var result = cmd.ExecuteScalar();

                if (result is null) result = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, };
                return (byte[])result;
            }
        }

        public void SaveLastChangedVersion(byte[] vesrsion, Guid pointId)
        {
            var lastVersion = vesrsion;

            //var tran = context.Connection.BeginTransaction(IsolationLevel.RepeatableRead);
            this.BeginTransaction(IsolationLevel.RepeatableRead);
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    @"DELETE FROM __Syncronization WHERE PointId = @PointId
                  INSERT INTO __Syncronization(PointId, SyncronizedVersion, SyncronizationDate)
                  VALUES(@PointId, @Version, @Date)";

                cmd.Parameters.Add(new SqlParameter("@PointId", pointId));
                cmd.Parameters.Add(new SqlParameter("@Version", lastVersion));
                cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now));

                cmd.ExecuteNonQuery();
            }
            CommitTransaction();
        }
        public void SaveLastUncommitedVersion(byte[] version, Guid pointId)
        {
            Logger.LogDebug($"Start saving uncommited version: {CommonHelper.ByteArrayToString(version)}");
            var lastVersion = version;

            BeginTransaction(IsolationLevel.RepeatableRead);
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    @"IF(EXISTS(SELECT 1 FROM __Syncronization WHERE PointId = @PointId))
                        UPDATE __Syncronization SET SyncronizationUncommitedVersion = @Version, UncommitedDate = @Date WHERE PointId = @PointId
                      ELSE 
                        INSERT INTO __Syncronization(UncommitedDate, PointId, SyncronizationUncommitedVersion) VALUES (@Date, @PointId, @Version)
";
                cmd.Parameters.Add(new SqlParameter("@PointId", pointId));
                cmd.Parameters.Add(new SqlParameter("@Version", lastVersion));
                cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now));

                cmd.ExecuteNonQuery();
            }
            CommitTransaction();
        }
        public byte[] GetLastUncommitedVersion(Guid pointId, int maxUncommitedTimeInSeconds)
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT SyncronizationUncommitedVersion FROM __Syncronization WHERE UncommitedDate > @Date AND PointId = @PointId";

                cmd.Parameters.Add(new SqlParameter("@PointId", pointId));
                cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now.AddSeconds(-maxUncommitedTimeInSeconds)));

                return (byte[])cmd.ExecuteScalar();
            }
        }

    }
}
