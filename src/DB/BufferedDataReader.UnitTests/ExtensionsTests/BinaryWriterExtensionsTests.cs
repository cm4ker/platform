using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using BufferedDataReaderDotNet.Extensions;
using Xunit;

namespace BufferedDataReaderDotNet.UnitTests.ExtensionsTests
{
    public class BinaryWriterExtensionsTests
    {
        [Fact]
        public void WriteBytes_WriteBytes()
        {
            var bytes = new byte[byte.MaxValue];
            var random = new Random();

            random.NextBytes(bytes);

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(bytes.Length);
                    binaryWriter.Write(bytes);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    Assert.Equal(bytes.Length, binaryReader.ReadInt32());
                    Assert.Equal(bytes, binaryReader.ReadBytes(bytes.Length));
                    Assert.Equal(memoryStream.Length, memoryStream.Position);
                }
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        [Fact]
        public void WriteDateTime_WritesDateTime()
        {
            var dateTime = DateTime.UtcNow;

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    binaryWriter.Write(dateTime.ToBinary());

                Assert.Equal(dateTime.ToBinary(), BitConverter.ToInt64(memoryStream.ToArray(), 0));
                Assert.Equal(memoryStream.Length, memoryStream.Position);
            }
        }

        [Fact]
        public void WriteGuid_WritesGuid()
        {
            var guid = Guid.NewGuid();

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    binaryWriter.Write(guid.ToByteArray());

                Assert.Equal(guid, new Guid(memoryStream.ToArray()));
                Assert.Equal(memoryStream.Length, memoryStream.Position);
            }
        }
    }
}