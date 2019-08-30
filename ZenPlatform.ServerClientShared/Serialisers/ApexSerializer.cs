using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZenPlatform.Core.Serialisers
{
    public class ApexSerializer : ISerializer
    {
        public T FromBytes<T>(byte[] input)
        {
            var binarySerializer = Binary.Create();
           return  binarySerializer.Read<T>(new MemoryStream(input));


        }

        public object FromBytes(byte[] input)
        {
            return FromBytes<object>(input);
        }

        public byte[] ToBytes<T>(T input)
        {
            var binarySerializer = Binary.Create();
            var stream = new MemoryStream();
            binarySerializer.Write(input, stream);

            return stream.ToArray();
        }
    }
}
