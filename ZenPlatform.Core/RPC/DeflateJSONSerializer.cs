using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace ZenPlatform.Core.RPC
{
    public class DeflateJSONSerializer : ISerializer
    {
        public T FromBytes<T>(byte[] input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            using (MemoryStream memoryStream = new MemoryStream(input))
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                {
                    StreamReader streamReader = new StreamReader(deflateStream);
                    return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd(), settings);
                }      
            }
        }

        public byte[] ToBytes<T>(T input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };

            using (var memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    using (StreamWriter streamWriter = new StreamWriter(deflateStream))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(input, settings));
                    }
                }
                return memoryStream.ToArray();
            }

        }
    }
}
