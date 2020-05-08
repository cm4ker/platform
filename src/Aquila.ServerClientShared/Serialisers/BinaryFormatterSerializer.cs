using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Aquila.Core.Serialisers
{
    public class BinaryFormatterSerializer: ISerializer
    {

        public T FromBytes<T>(byte[] input)
        {
            return (T)FromBytes(input);
        }

        public object FromBytes(byte[] input)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat
                = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            return formatter.Deserialize(new MemoryStream(input));
        }

        public byte[] ToBytes<T>(T input)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat
                = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            var stream = new MemoryStream();
            formatter.Serialize(stream, input);

            return stream.ToArray();
        }
    }
}
