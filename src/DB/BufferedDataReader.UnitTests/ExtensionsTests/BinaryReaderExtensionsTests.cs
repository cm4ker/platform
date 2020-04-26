using System;
using System.IO;
using System.Text;
using BufferedDataReaderDotNet.Extensions;
using Xunit;

namespace BufferedDataReaderDotNet.UnitTests.ExtensionsTests
{
    public class BinaryReaderExtensionsTests
    {
        [Fact]
        public void ReadBytes_ReadsBytes()
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
                    Assert.Equal(bytes, binaryReader.ReadBytes());
                    Assert.Equal(memoryStream.Length, memoryStream.Position);
                }
            }
        }

        [Fact]
        public void ReadDateTime_ReadsDateTime()
        {
            var dateTime = DateTime.UtcNow;
            var binaryDateTime = dateTime.ToBinary();

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    binaryWriter.Write(binaryDateTime);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    Assert.Equal(dateTime, binaryReader.ReadDateTime());
                    Assert.Equal(memoryStream.Length, memoryStream.Position);
                }
            }
        }

        [Fact]
        public void ReadGuid_ReadsGuid()
        {
            var guid = Guid.NewGuid();
            var guidBytes = guid.ToByteArray();

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    binaryWriter.Write(guidBytes);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    Assert.Equal(guid, binaryReader.ReadGuid());
                    Assert.Equal(memoryStream.Length, memoryStream.Position);
                }
            }
        }
    }
}