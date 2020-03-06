using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.Core.Network;
using System.IO;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.ServerRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection con = new SqlConnection();

            con.ConnectionString =
                "Data source=PC702\\Asna; Initial catalog=asna_apt_019; User Id=sa; Password=sapwd123;"; // MultipleActiveResultSets=True";

            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "select * from sys.tables";
            var reader1 = cmd.ExecuteReader();

            var cmd2 = con.CreateCommand();
            cmd2.CommandText = "SElect * from sys.tables";
            var reader2 = cmd2.ExecuteReader();
        }
    }


    public class FreeDbDataReader : IDisposable, IDataReader
    {
        private readonly DbCommand _command;

        public FreeDbDataReader(DbCommand command)
        {
            _command = command;
        }


        public void Dispose()
        {
            _command?.Dispose();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public int FieldCount { get; }

        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        private List<object> buffer = new List<object>();
        private int _cPos = 0;

        public bool Read()
        {
        }

        private void FetchNextSet(object symantic, object builder)
        {
            // There we need some context
        }

        public int Depth { get; }
        public bool IsClosed { get; }
        public int RecordsAffected { get; }
    }
}