using System;
using System.IO;

namespace BufferedDataReaderDotNet.Infrastructure
{
    internal sealed class ColumnWriter : IDisposable
    {
        private readonly BufferedStream _bufferedStream;
        private readonly CompressingStream _compressingStream;

        public ColumnWriter(Stream innerStream)
        {
            _compressingStream = new CompressingStream(innerStream);
            _bufferedStream = new BufferedStream(_compressingStream, ushort.MaxValue);

            BinaryWriter = new BinaryWriter(_bufferedStream);
        }

        public BinaryWriter BinaryWriter { get; }

        public void Dispose()
        {
            BinaryWriter.Dispose();
            _bufferedStream.Dispose();
            _compressingStream.Dispose();
        }
    }
}