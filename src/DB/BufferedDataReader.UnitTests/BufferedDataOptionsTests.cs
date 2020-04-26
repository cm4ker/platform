using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace BufferedDataReaderDotNet.UnitTests
{
    public class BufferedDataOptionsTests
    {
        [Fact]
        public void BufferedDataOptions_CopiesReadFuncs()
        {
            var readFuncs = new Dictionary<Type, Func<BinaryReader, object>>
            {
                {typeof(BufferedDataOptionsTests), null}
            };

            var bufferedDataOptions = new BufferedDataOptions(readFuncs: readFuncs);

            Assert.True(bufferedDataOptions.ReadFuncs.ContainsKey(typeof(BufferedDataOptionsTests)));

            readFuncs.Clear();

            Assert.True(bufferedDataOptions.ReadFuncs.ContainsKey(typeof(BufferedDataOptionsTests)));
        }

        [Fact]
        public void BufferedDataOptions_CopiesWriteActions()
        {
            var writeActions = new Dictionary<Type, Action<object, BinaryWriter>>()
            {
                {typeof(BufferedDataOptionsTests), null}
            };

            var bufferedDataOptions = new BufferedDataOptions(writeActions: writeActions);

            Assert.True(bufferedDataOptions.WriteActions.ContainsKey(typeof(BufferedDataOptionsTests)));

            writeActions.Clear();

            Assert.True(bufferedDataOptions.WriteActions.ContainsKey(typeof(BufferedDataOptionsTests)));
        }

        [Fact]
        public void BufferedDataOptions_ProvidesDefaults()
        {
            var bufferedDataOptions = new BufferedDataOptions();

            Assert.NotNull(bufferedDataOptions.GetCompressedStream());
            Assert.NotNull(bufferedDataOptions.ReadFuncs);
            Assert.NotNull(bufferedDataOptions.WriteActions);

            using (var compressedStream = bufferedDataOptions.GetCompressedStream())
                Assert.NotNull(compressedStream);
        }
    }
}