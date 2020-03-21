using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BufferedDataReaderDotNet.Extensions
{
    internal static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream sourceStream,
            Stream destinationStream, long length, CancellationToken cancellationToken)
        {
            // CopyToAsync on MSDN: "bufferSize ... [t]he default size is 81920."
            var buffer = new byte[81920];

            while (length > 0)
            {
                var count = (int) Math.Min(buffer.Length, length);
                var bytesRead = await sourceStream.ReadAsync(buffer, 0, count, cancellationToken);

                await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                length -= bytesRead;
            }
        }
    }
}