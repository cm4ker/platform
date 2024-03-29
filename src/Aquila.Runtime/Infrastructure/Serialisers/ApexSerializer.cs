﻿using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aquila.Core.Serialisers
{
    public class ApexSerializer : ISerializer
    {
        public T FromBytes<T>(byte[] input)
        {
            var binarySerializer = Binary.Create(new Apex.Serialization.Settings().MarkSerializable((t) => true));
            return binarySerializer.Read<T>(new MemoryStream(input));
        }

        public object FromBytes(byte[] input)
        {
            return FromBytes<object>(input);
        }

        public byte[] ToBytes<T>(T input)
        {
            var binarySerializer = Binary.Create(new Apex.Serialization.Settings().MarkSerializable((t) => true));
            var stream = new MemoryStream();
            binarySerializer.Write(input, stream);

            return stream.ToArray();
        }
    }
}