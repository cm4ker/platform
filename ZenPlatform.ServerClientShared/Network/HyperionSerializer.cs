using System;
using System.Collections.Generic;
using System.Text;
using Hyperion.SerializerFactories;
using Hyperion;
using System.IO;

namespace ZenPlatform.ServerClientShared.Network
{
    public class HyperionSerializer : ISerializer
    {
        private readonly Serializer _serializer;

        public HyperionSerializer()
        {
            _serializer = new Serializer(new SerializerOptions());
        }

        public T FromBytes<T>(byte[] input)
        {
            return _serializer.Deserialize<T>(new MemoryStream(input));
        }

        public object FromBytes(byte[] input)
        {
            return _serializer.Deserialize(new MemoryStream(input));
        }

        public byte[] ToBytes<T>(T input)
        {
            var stream = new MemoryStream();

            _serializer.Serialize(input, stream);

            return stream.ToArray();
        }
    }
}