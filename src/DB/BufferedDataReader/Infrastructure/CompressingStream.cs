using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BufferedDataReaderDotNet.Infrastructure
{
    internal class CompressingStream : Stream
    {
        private readonly ActionBlock<byte[]> _actionBlock;
        private readonly GZipStream _gzipStream;

        public CompressingStream(Stream innerStream)
        {
            _actionBlock = new ActionBlock<byte[]>(WriteByteArrayAsync);
            _gzipStream = new GZipStream(innerStream, CompressionMode.Compress, true);
        }

        public override bool CanRead => false;

        /// <remarks>
        ///     "If a class ... does not support seeking, calls to <see cref="Length" />, <see cref="SetLength" />,
        ///     <see cref="Position" />, and <see cref="Seek" /> throw a <see cref="NotSupportedException" />."
        /// </remarks>
        /// <see>
        ///     https://msdn.microsoft.com/en-us/library/system.io.stream.canseek.aspx
        /// </see>
        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        private Task WriteByteArrayAsync(byte[] byteArray)
        {
            return byteArray == null ? _gzipStream.FlushAsync() : _gzipStream.WriteAsync(byteArray, 0, byteArray.Length);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Flush();

                _actionBlock.Complete();
                _actionBlock.Completion.Wait();

                _gzipStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Flush()
        {
            _actionBlock.Post(null);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytes = new byte[count];

            Buffer.BlockCopy(buffer, offset, bytes, 0, count);

            _actionBlock.Post(bytes);
        }
    }
}