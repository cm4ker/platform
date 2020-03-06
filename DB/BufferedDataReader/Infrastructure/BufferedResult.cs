using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Extensions;

namespace BufferedDataReaderDotNet.Infrastructure
{
    internal sealed class BufferedResult : IDisposable, IAsyncSerializable
    {
        // TODO: VisibleFieldCount
        internal BufferedResult(
            DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions,
            IReadOnlyList<Stream> compressedStreams,
            int recordCount)
            : this(
                bufferedDataOptions,
                compressedStreams,
                Get(dataReader.FieldCount, dataReader.GetDataTypeName),
                dataReader.Depth,
                dataReader.FieldCount,
                Get(dataReader.FieldCount, dataReader.GetFieldType),
                dataReader.HasRows,
                Get(dataReader.FieldCount, dataReader.GetName),
                recordCount,
                dataReader.RecordsAffected,
                dataReader.GetSchemaTable())
        {
        }

        private BufferedResult(
            BufferedDataOptions bufferedDataOptions,
            IReadOnlyList<Stream> compressedStreams,
            IReadOnlyList<string> dataTypeNames,
            int depth,
            int fieldCount,
            IReadOnlyList<Type> fieldTypes,
            bool hasRows,
            IReadOnlyList<string> names,
            int recordCount,
            int recordsAffected,
            DataTable schemaTable)
        {
            BufferedDataOptions = bufferedDataOptions;
            CompressedStreams = compressedStreams;
            DataTypeNames = dataTypeNames;
            Depth = depth;
            FieldCount = fieldCount;
            FieldTypes = fieldTypes;
            HasRows = hasRows;
            Names = names;
            RecordCount = recordCount;
            RecordsAffected = recordsAffected;
            SchemaTable = schemaTable;
        }

        public BufferedDataOptions BufferedDataOptions { get; }

        public IReadOnlyList<Stream> CompressedStreams { get; }

        public IReadOnlyList<string> DataTypeNames { get; }

        public int Depth { get; }

        public int FieldCount { get; }

        public IReadOnlyList<Type> FieldTypes { get; }

        public bool HasRows { get; }

        public IReadOnlyList<string> Names { get; }

        // TODO: Find some way to expose this
        public int RecordCount { get; }

        public int RecordsAffected { get; }

        public DataTable SchemaTable { get; }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                binaryWriter.WriteDataTable(SchemaTable);
                binaryWriter.WriteStrings(DataTypeNames);
                binaryWriter.Write(Depth);
                binaryWriter.Write(FieldCount);
                binaryWriter.WriteTypes(FieldTypes);
                binaryWriter.Write(HasRows);
                binaryWriter.WriteStrings(Names);
                binaryWriter.Write(RecordCount);
                binaryWriter.Write(RecordsAffected);
                binaryWriter.Write(CompressedStreams.Count);

                foreach (var compressedStream in CompressedStreams)
                {
                    compressedStream.Seek(0, SeekOrigin.End);
                    binaryWriter.Write(compressedStream.Position);
                    compressedStream.Seek(0, SeekOrigin.Begin);
                }
            }

            foreach (var compressedStream in CompressedStreams)
            {
                // CopyToAsync on MSDN: "bufferSize ... [t]he default size is 81920."
                await compressedStream.CopyToAsync(stream, 81920, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            foreach (var compressedStream in CompressedStreams)
                compressedStream.Dispose();
        }

        private static IReadOnlyList<TValue> Get<TValue>(int fieldCount, Func<int, TValue> valueFunc)
        {
            var values = new List<TValue>(fieldCount);

            for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                values.Add(valueFunc(fieldIndex));

            return values;
        }

        public static async Task<BufferedResult> ReadFromAsync(
            Stream stream, BufferedDataOptions bufferedDataOptions, CancellationToken cancellationToken)
        {
            // NB: IDisposable but in practice safe to ignore this.
            DataTable schemaTable;
            IReadOnlyList<string> dataTypeNames;
            int depth;
            int fieldCount;
            IReadOnlyList<Type> fieldTypes;
            bool hasRows;
            IReadOnlyList<string> names;
            int recordCount;
            int recordsAffected;
            List<long> compressedStreamLengths;

            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                schemaTable = binaryReader.ReadDataTable();
                dataTypeNames = binaryReader.ReadStrings();
                depth = binaryReader.ReadInt32();
                fieldCount = binaryReader.ReadInt32();
                fieldTypes = binaryReader.ReadTypes();
                hasRows = binaryReader.ReadBoolean();
                names = binaryReader.ReadStrings();
                recordCount = binaryReader.ReadInt32();
                recordsAffected = binaryReader.ReadInt32();
                compressedStreamLengths = binaryReader.ReadInt64s();
            }

            var compressedStreams = new List<Stream>(compressedStreamLengths.Count);

            try
            {
                foreach (var compressedStreamLength in compressedStreamLengths)
                {
                    var compressedStream = bufferedDataOptions.GetCompressedStream();

                    compressedStreams.Add(compressedStream);

                    var copyToTask = stream.CopyToAsync(
                        compressedStream, compressedStreamLength, cancellationToken);

                    await copyToTask.ConfigureAwait(false);
                }

                return new BufferedResult(bufferedDataOptions, compressedStreams, dataTypeNames, depth,
                    fieldCount, fieldTypes, hasRows, names, recordCount, recordsAffected, schemaTable);
            }
            catch
            {
                foreach (var compressedStream in compressedStreams)
                    compressedStream.Dispose();

                throw;
            }
        }
    }
}