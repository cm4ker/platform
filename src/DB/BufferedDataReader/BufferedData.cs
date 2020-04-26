using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Infrastructure;
using JetBrains.Annotations;

namespace BufferedDataReaderDotNet
{
    public sealed class BufferedData : IDisposable, IAsyncSerializable
    {
        private const int BinaryVersion = 1;

        private readonly Queue<BufferedResult> _bufferedResults;

        internal BufferedData(Queue<BufferedResult> bufferedResults)
        {
            _bufferedResults = bufferedResults;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                binaryWriter.Write(BinaryVersion);
                binaryWriter.Write(_bufferedResults.Count);
            }

            foreach (var bufferedResult in _bufferedResults)
                await bufferedResult.WriteToAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            foreach (var bufferedResult in _bufferedResults)
                bufferedResult.Dispose();
        }

        [PublicAPI]
        public BufferedDataReader GetDataReader()
        {
            return new BufferedDataReader(_bufferedResults);
        }

        public static async Task<BufferedData> ReadFromAsync(
            Stream stream, BufferedDataOptions bufferedDataOptions, CancellationToken cancellationToken)
        {
            var bufferedResults = new Queue<BufferedResult>();
            int bufferedResultsCount;

            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var version = binaryReader.ReadInt32();

                if (version != BinaryVersion)
                    throw new ArgumentOutOfRangeException(nameof(stream), $"Unexpected {nameof(BinaryVersion)}");

                bufferedResultsCount = binaryReader.ReadInt32();
            }

            for (; bufferedResultsCount > 0; --bufferedResultsCount)
            {
                var bufferedResultTask = BufferedResult.ReadFromAsync(stream, bufferedDataOptions, cancellationToken);
                var bufferedResult = await bufferedResultTask.ConfigureAwait(false);

                bufferedResults.Enqueue(bufferedResult);
            }

            return new BufferedData(bufferedResults);
        }
    }
}