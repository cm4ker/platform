using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace ZenPlatform.ServerClientShared.Network
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public T FromBytes<T>(byte[] input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                
            };

            var data = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject<T>(data);
        }

        public object FromBytes(byte[] input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
            };

            var data = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject(data);
            /*
            using (MemoryStream memoryStream = new MemoryStream(input))
            {

                StreamReader streamReader = new StreamReader(memoryStream);
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetBytes(data);streamReader.ReadToEnd(), settings);

            }
            */
        }

        public byte[] ToBytes<T>(T input)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            /*
            using (var memoryStream = new MemoryStream())
            {

                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write();
                }
                */
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(input, settings)); 
           // }

        }

    
    }
}
