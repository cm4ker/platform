using System;
using System.Collections.Generic;
using System.Data.Common;
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
        public static async Task<BufferedData> GetBufferedDataAsync(
            this DbDataReader dataReader, BufferedDataOptions bufferedDataOptions, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (bufferedDataOptions == null)
                bufferedDataOptions = BufferedDataOptions.Default;

            var bufferedResults = new Queue<BufferedResult>();

            try
            {
                var getBufferedResultsTask = GetBufferedResultsAsync(
                    dataReader, bufferedDataOptions,bufferedResults, cancellationToken);

                await getBufferedResultsTask.ConfigureAwait(false);

                return new BufferedData(bufferedResults);
            }
            catch
            {
                foreach (var bufferedResult in bufferedResults)
                    bufferedResult.Dispose();

                throw;
            }
        }

        private static async Task GetBufferedResultsAsync(
            DbDataReader dataReader, BufferedDataOptions bufferedDataOptions,
            Queue<BufferedResult> bufferedResults, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fieldCount = dataReader.FieldCount;
            var compressedStreams = new List<Stream>(fieldCount);

            try
            {
                for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                    compressedStreams.Add(bufferedDataOptions.GetCompressedStream());

                var getBufferedResultsTask = GetBufferedResultsAsync(dataReader,
                    bufferedDataOptions, bufferedResults, compressedStreams, cancellationToken);

                await getBufferedResultsTask.ConfigureAwait(false);
            }
            catch
            {
                foreach (var compressedStream in compressedStreams)
                    compressedStream.Dispose();

                throw;
            }
        }

        private static async Task GetBufferedResultsAsync(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fieldCount = dataReader.FieldCount;
            var columnWriters = new List<ColumnWriter>(fieldCount);

            try
            {
                for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                    columnWriters.Add(new ColumnWriter(compressedStreams[fieldIndex]));

                var getBufferedResultsTask = GetBufferedResultsAsync(dataReader,
                    bufferedDataOptions,bufferedResults, compressedStreams, columnWriters, cancellationToken);

                await getBufferedResultsTask.ConfigureAwait(false);
            }
            finally
            {
                foreach (var columnStream in columnWriters)
                    columnStream.Dispose();
            }
        }

        private static async Task GetBufferedResultsAsync(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams, List<ColumnWriter> columnWriters,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fieldCount = dataReader.FieldCount;
            var writeActions = new Action<object, BinaryWriter>[fieldCount];

            for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
                writeActions[fieldIndex] = dataReader.GetFieldType(fieldIndex).GetWriteAction(bufferedDataOptions);

            var recordCount = 0;
            var values = new object[columnWriters.Capacity];

            for (; await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false); ++recordCount)
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

            var getBufferedResultsTask = GetBufferedResultsAsync(dataReader,
                bufferedDataOptions, bufferedResults, compressedStreams, recordCount, cancellationToken);

            await getBufferedResultsTask.ConfigureAwait(false);
        }

        private static async Task GetBufferedResultsAsync(DbDataReader dataReader,
            BufferedDataOptions bufferedDataOptions, Queue<BufferedResult> bufferedResults,
            IReadOnlyList<Stream> compressedStreams, int recordCount, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bufferedResult = new BufferedResult(dataReader, bufferedDataOptions, compressedStreams, recordCount);

            try
            {
                bufferedResults.Enqueue(bufferedResult);

                if (await dataReader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                {
                    var getBufferedResultsTask = GetBufferedResultsAsync(
                        dataReader, bufferedDataOptions, bufferedResults, cancellationToken);

                    await getBufferedResultsTask.ConfigureAwait(false);
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