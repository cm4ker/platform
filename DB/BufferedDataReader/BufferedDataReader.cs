using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Infrastructure;
using JetBrains.Annotations;

namespace BufferedDataReaderDotNet
{
    public sealed class BufferedDataReader : DbDataReader
    {
        private readonly Queue<BufferedResult> _bufferedResults;

        [CanBeNull] private BufferedResultReader _bufferedResultReader;

        internal BufferedDataReader(Queue<BufferedResult> bufferedResults)
        {
            _bufferedResults = bufferedResults;

            NextResult();
        }

        public override int FieldCount => _bufferedResultReader?.FieldCount ?? -1;

        public override object this[int ordinal] => _bufferedResultReader?[ordinal];

        public override object this[string name] => _bufferedResultReader?[name];

        public override bool HasRows => _bufferedResultReader?.HasRows ?? false;

        public override bool IsClosed => _bufferedResultReader?.IsClosed ?? true;

        public int RecordCount => _bufferedResultReader.RecordCount;

        // TODO: Close our data reader
        /// <remarks>"The RecordsAffected property is not set until all rows are read and you close the DataReader."</remarks>
        /// <see>https://msdn.microsoft.com/en-us/library/system.data.common.dbdatareader.recordsaffected(v=vs.110).aspx</see>
        public override int RecordsAffected { get; }

        public override int Depth => _bufferedResultReader.Depth;

        public override string GetName(int ordinal) => _bufferedResultReader?.GetName(ordinal);

        public override int GetValues(object[] values) => _bufferedResultReader?.GetValues(values) ?? 0;

        public override bool IsDBNull(int ordinal) => _bufferedResultReader.IsDBNull(ordinal);

        public override void Close() => _bufferedResultReader?.Close();

        public override DataTable GetSchemaTable() => _bufferedResultReader?.GetSchemaTable();

        public override bool NextResult()
        {
            if (_bufferedResults.Count > 0)
            {
                _bufferedResultReader = new BufferedResultReader(_bufferedResults.Dequeue());

                return true;
            }

            _bufferedResultReader = null;

            return false;
        }

        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();

            return NextResult();
        }

        public override bool Read() => _bufferedResultReader.Read();

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            _bufferedResultReader?.ReadAsync(cancellationToken);

        public override int GetOrdinal(string name) =>
            _bufferedResultReader?.GetOrdinal(name) ?? throw new NullReferenceException();

        public override bool GetBoolean(int ordinal) =>
            _bufferedResultReader?.GetBoolean(ordinal) ?? throw new NullReferenceException();

        public override byte GetByte(int ordinal) =>
            _bufferedResultReader?.GetByte(ordinal) ?? throw new NullReferenceException();

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal) =>
            _bufferedResultReader?.GetChar(ordinal) ?? throw new NullReferenceException();

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal) =>
            _bufferedResultReader?.GetGuid(ordinal) ?? throw new NullReferenceException();

        public override short GetInt16(int ordinal) =>
            _bufferedResultReader?.GetInt16(ordinal) ?? throw new NullReferenceException();

        public override int GetInt32(int ordinal) =>
            _bufferedResultReader?.GetInt32(ordinal) ?? throw new NullReferenceException();

        public override long GetInt64(int ordinal) =>
            _bufferedResultReader?.GetChar(ordinal) ?? throw new NullReferenceException();

        public override DateTime GetDateTime(int ordinal) =>
            _bufferedResultReader?.GetDateTime(ordinal) ?? throw new NullReferenceException();

        public override string GetString(int ordinal) =>
            _bufferedResultReader?.GetString(ordinal);

        public override decimal GetDecimal(int ordinal) =>
            _bufferedResultReader?.GetDecimal(ordinal) ?? throw new NullReferenceException();

        public override double GetDouble(int ordinal) =>
            _bufferedResultReader?.GetDouble(ordinal) ?? throw new NullReferenceException();

        public override float GetFloat(int ordinal) =>
            _bufferedResultReader?.GetFloat(ordinal) ?? throw new NullReferenceException();

        public override string GetDataTypeName(int ordinal) => _bufferedResultReader?.GetDataTypeName(ordinal);

        public override Type GetFieldType(int ordinal) => _bufferedResultReader?.GetFieldType(ordinal);

        public override object GetValue(int ordinal) => _bufferedResultReader?.GetValue(ordinal);

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}