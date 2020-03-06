using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace BufferedDataReaderDotNet
{
    public sealed class BufferedDataOptions
    {
        private static readonly Func<Stream> DefaultCompressedStreamFunc;
        private static readonly IReadOnlyDictionary<Type, Func<BinaryReader, object>> DefaultReadFuncs;
        private static readonly IReadOnlyDictionary<Type, Action<object, BinaryWriter>> DefaultWriteActions;

        private readonly Func<Stream> _compressedStreamFunc;

        static BufferedDataOptions()
        {
            var emptyReadFuncs = new Dictionary<Type, Func<BinaryReader, object>>();
            var emptyWriteOptions = new Dictionary<Type, Action<object, BinaryWriter>>();

            DefaultCompressedStreamFunc = () => new MemoryStream();
            DefaultReadFuncs = new ReadOnlyDictionary<Type, Func<BinaryReader, object>>(emptyReadFuncs);
            DefaultWriteActions = new ReadOnlyDictionary<Type, Action<object, BinaryWriter>>(emptyWriteOptions);

            Default = new BufferedDataOptions();
        }

        public BufferedDataOptions(Func<Stream> compressedStreamFunc = null,
            IDictionary<Type, Func<BinaryReader, object>> readFuncs = null,
            IDictionary<Type, Action<object, BinaryWriter>> writeActions = null,
            int yieldInterval = 100)
        {
            _compressedStreamFunc = compressedStreamFunc ?? DefaultCompressedStreamFunc;

            if (readFuncs != null)
            {
                ReadFuncs = new ReadOnlyDictionary<Type, Func<BinaryReader, object>>(
                    readFuncs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            }
            else
            {
                ReadFuncs = DefaultReadFuncs;
            }


            if (writeActions != null)
            {
                WriteActions = new ReadOnlyDictionary<Type, Action<object, BinaryWriter>>(
                    writeActions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            }
            else
            {
                WriteActions = DefaultWriteActions;
            }

            YieldInterval = yieldInterval;
        }

        public static BufferedDataOptions Default { get; }

        public IReadOnlyDictionary<Type, Func<BinaryReader, object>> ReadFuncs { get; }

        public IReadOnlyDictionary<Type, Action<object, BinaryWriter>> WriteActions { get; }

        public int YieldInterval { get; }

        public Stream GetCompressedStream()
        {
            Stream compressedStream = null;

            try
            {
                compressedStream = _compressedStreamFunc();

                if (!compressedStream.CanSeek)
                {
                    compressedStream.Dispose();

                    throw new ArgumentOutOfRangeException(nameof(_compressedStreamFunc));
                }

                return compressedStream;
            }
            catch
            {
                compressedStream?.Dispose();

                throw;
            }
        }
    }
}