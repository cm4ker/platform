using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Extensions;

namespace BufferedDataReaderDotNet.Infrastructure
{
    internal sealed class BufferedResultReader : DbDataReader
    {
        private readonly IReadOnlyList<BinaryReader> _binaryReaders;
        private readonly BufferedResult _bufferedResult;
        private readonly IReadOnlyList<Stream> _decompressingStreams;
        private readonly Func<BinaryReader, object>[] _readFuncs;
        private readonly object[] _values;

        private int _currentRecord = -1;
        private bool _isClosed;

        public BufferedResultReader(BufferedResult bufferedResult)
        {
            _bufferedResult = bufferedResult;
            _values = new object[FieldCount];

            _decompressingStreams = GetDecompressingStreams();
            _binaryReaders = GetBinaryReaders();
            _readFuncs = GetReadFuncs();
        }

        public override int FieldCount => _bufferedResult.FieldCount;

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => this[GetOrdinal(name)];

        public override bool HasRows => _bufferedResult.HasRows;

        public override bool IsClosed => _isClosed;

        // TODO: Some way to expose this
        // Probably put BufferedDataReader into the namespace one up from "inf" and make public
        public int RecordCount => _bufferedResult.RecordCount;

        public override int RecordsAffected { get; }

        public override int Depth { get; }

        private IReadOnlyList<Stream> GetDecompressingStreams()
        {
            var compressedStreams = _bufferedResult.CompressedStreams;
            var decompressingStreams = new List<Stream>(compressedStreams.Count);

            try
            {
                foreach (var compressedStream in compressedStreams)
                {
                    // Note this side effect means that multiple BufferedResultReaders can't
                    // read from the same data concurrently. They can only be used serially.
                    compressedStream.Position = 0;

                    decompressingStreams.Add(new GZipStream(compressedStream, CompressionMode.Decompress, true));
                }
            }
            catch
            {
                foreach (var decompressingStream in decompressingStreams)
                    decompressingStream.Dispose();

                throw;
            }

            return decompressingStreams;
        }

        private IReadOnlyList<BinaryReader> GetBinaryReaders()
        {
            var binaryReaders = new List<BinaryReader>(_decompressingStreams.Count);

            try
            {
                foreach (var decompressingStream in _decompressingStreams)
                    binaryReaders.Add(new BinaryReader(decompressingStream, Encoding.UTF8, true));

                return binaryReaders;
            }
            catch
            {
                foreach (var binaryReader in binaryReaders)
                    binaryReader.Dispose();

                throw;
            }
        }

        private Func<BinaryReader, object>[] GetReadFuncs()
        {
            var readFuncs = new Func<BinaryReader, object>[FieldCount];

            for (var fieldIndex = 0; fieldIndex < FieldCount; ++fieldIndex)
                readFuncs[fieldIndex] = GetFieldType(fieldIndex).GetReadFunc(_bufferedResult.BufferedDataOptions);

            return readFuncs;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var binaryReader in _binaryReaders)
                    binaryReader.Dispose();

                foreach (var decompressingStream in _decompressingStreams)
                    decompressingStream.Dispose();

                _isClosed = true;
            }
        }

        public override string GetName(int ordinal) => _bufferedResult.Names[ordinal];

        public override void Close()
        {
            _isClosed = true;
        }

        public override DataTable GetSchemaTable() => _bufferedResult.SchemaTable;

        public override int GetValues(object[] values)
        {
            var length = Math.Min(_values.Length, values.Length);

            Array.Copy(_values, values, length);

            return length;
        }

        public override bool IsDBNull(int ordinal)
        {
            return Convert.IsDBNull(_values[ordinal]);
        }

        public override bool NextResult() => false;

        public override bool Read()
        {
            if (++_currentRecord < RecordCount)
            {
                for (var fieldIndex = 0; fieldIndex < FieldCount; ++fieldIndex)
                {
                    var readFunc = _readFuncs[fieldIndex];
                    var binaryReader = _binaryReaders[fieldIndex];
                    var hasValue = binaryReader.ReadBoolean();

                    _values[fieldIndex] = hasValue ? readFunc(binaryReader) : DBNull.Value;
                }

                return true;
            }

            return false;
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bufferedDataOptions = _bufferedResult.BufferedDataOptions;
            var yieldInterval = bufferedDataOptions.YieldInterval;

            if (yieldInterval > 0 && _currentRecord % yieldInterval == 0)
                await Task.Yield();

            return Read();
        }

        public override int GetOrdinal(string name)
        {
            return _bufferedResult.Names.IndexOf(name);
        }

        public override bool GetBoolean(int ordinal) => (bool)_values[ordinal];

        public override byte GetByte(int ordinal) => (byte)_values[ordinal];

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal) => (char)_values[ordinal];

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal) => (Guid)_values[ordinal];

        public override short GetInt16(int ordinal) => (short)_values[ordinal];

        public override int GetInt32(int ordinal) => (int)_values[ordinal];

        public override long GetInt64(int ordinal) => (long)_values[ordinal];

        public override DateTime GetDateTime(int ordinal) => (DateTime)_values[ordinal];

        public override string GetString(int ordinal) => (string)_values[ordinal];

        public override decimal GetDecimal(int ordinal) => (decimal)_values[ordinal];

        public override double GetDouble(int ordinal) => (double)_values[ordinal];

        public override float GetFloat(int ordinal) => (float)_values[ordinal];

        public override string GetDataTypeName(int ordinal) => _bufferedResult.DataTypeNames[ordinal];

        public override Type GetFieldType(int ordinal) => _bufferedResult.FieldTypes[ordinal];

        public override object GetValue(int ordinal) => _values[ordinal];

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }
    }
}