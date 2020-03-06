using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Extensions;
using BufferedDataReaderDotNet.Infrastructure;
using JetBrains.Annotations;

namespace BufferedDataReaderDotNet
{
    public static class DbDataReaderExtensions
    {
        [PublicAPI]
        public static Task<BufferedData> GetBufferedDataAsync(
            this DbDataReader dataReader, CancellationToken cancellationToken)
        {
            return GetBufferedDataAsync(dataReader, null, cancellationToken);
        }

        [PublicAPI]
        public static Task<BufferedData> GetBufferedDataAsync(
            this DbDataReader dataReader, BufferedDataOptions bufferedDataOptions, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (bufferedDataOptions == null)
                bufferedDataOptions = BufferedDataOptions.Default;
            try
            {
                return Task.FromResult(GetBufferedData(dataReader, bufferedDataOptions));
            }
            catch (Exception e)
            {
                return Task.FromException<BufferedData>(e);
            }
        }

        [PublicAPI]
        public static BufferedData GetBufferedData(this DbDataReader dataReader)
        {
            return GetBufferedData(dataReader, null);
        }

        [PublicAPI]
        public static BufferedData GetBufferedData(
            this DbDataReader dataReader, BufferedDataOptions bufferedDataOptions)
        {
            if (bufferedDataOptions == null)
                bufferedDataOptions = BufferedDataOptions.Default;

            var bufferedResults = new Queue<BufferedResult>();

            try
            {
                GetBufferedResults(dataReader, bufferedDataOptions, bufferedResults);
                return new BufferedData(bufferedResults);
            }
            catch
            {
                foreach (var bufferedResult in bufferedResults)
                    bufferedResult.Dispose();

                throw;
            }
        }

        private static void GetBufferedResults(DbDataReader dataReader, BufferedDataOptions bufferedDataOptions,
            Queue<BufferedResult> bufferedResults)
        {
            var fieldCount = dataReader.FieldCount;
            var compressedStreams = new List<Stream>(fieldCount);

            try
            {
                for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                    compressedStreams.Add(bufferedDataOptions.GetCompressedStream());

                GetBufferedResults(dataReader, bufferedDataOptions, bufferedResults, compressedStreams);
            }
            catch
            {
                foreach (var compressedStream in compressedStreams)
                    compressedStream.Dispose();

                throw;
            }
        }

        private static void GetBufferedResults(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams)
        {
            var fieldCount = dataReader.FieldCount;
            var columnWriters = new List<ColumnWriter>(fieldCount);

            try
            {
                for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                    columnWriters.Add(new ColumnWriter(compressedStreams[fieldIndex]));

                GetBufferedResults(dataReader, bufferedDataOptions, bufferedResults, compressedStreams,
                    columnWriters);
            }
            finally
            {
                foreach (var columnStream in columnWriters)
                    columnStream.Dispose();
            }
        }

        private static void GetBufferedResults(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams, List<ColumnWriter> columnWriters)
        {
            var fieldCount = dataReader.FieldCount;
            var writeActions = new Action<object, BinaryWriter>[fieldCount];

            for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                writeActions[fieldIndex] = dataReader.GetFieldType(fieldIndex).GetWriteAction(bufferedDataOptions);

            var recordCount = 0;
            var values = new object[columnWriters.Capacity];

            for (; dataReader.Read(); ++recordCount)
            {
                dataReader.GetValues(values);

                for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                {
                    var value = values[fieldIndex];
                    var binaryWriter = columnWriters[fieldIndex].BinaryWriter;
                    var hasValue = !Convert.IsDBNull(value);

                    binaryWriter.Write(hasValue);

                    if (hasValue)
                        writeActions[fieldIndex](value, binaryWriter);
                }
            }

            GetBufferedResults(dataReader, bufferedDataOptions, bufferedResults,
                compressedStreams, recordCount);
        }

        private static void GetBufferedResults(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams, int recordCount)
        {
            var bufferedResult = new BufferedResult(dataReader, bufferedDataOptions, compressedStreams, recordCount);

            try
            {
                bufferedResults.Enqueue(bufferedResult);

                if (dataReader.NextResult())
                {
                    GetBufferedResults(dataReader, bufferedDataOptions, bufferedResults);
                }
            }
            catch
            {
                bufferedResult.Dispose();

                throw;
            }
        }
    }
}