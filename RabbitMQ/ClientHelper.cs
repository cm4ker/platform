using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace RabbitMQClient
{
    public static class ClientHelper
    {
        public static T Is<T>(this object obj)
        {
            if (obj is T) return (T)obj;
            else return default(T);
        }

        public static void Is<T>(this object obj, Action<T> act)
        {
            if (obj is T) act((T)obj);
            else return;
        }

        //public static byte[] Pack(object obj)
        //{
        //    var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        //    var data = Encoding.ASCII.GetBytes(Compress(JsonConvert.SerializeObject(obj, settings)));
        //    return data;
        //}
        //public static T Unpack<T>(byte[] data)
        //{
        //    var str = Encoding.ASCII.GetString(data);
        //    var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        //    return (T)JsonConvert.DeserializeObject(Decompress(str), settings);
        //}
        //public static byte[] SerializeBson(object o)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    using (BsonWriter writer = new BsonWriter(ms))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        serializer.Serialize(writer, o);
        //    }
        //    return ms.ToArray();
        //}
        //public static T DeserializeBson<T>(byte[] data)
        //{
        //    MemoryStream ms = new MemoryStream(data);
        //    using (BsonReader reader = new BsonReader(ms))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();

        //        return serializer.Deserialize<T>(reader);
        //    }
        //}
        //public static object DeserializeBson(byte[] data)
        //{
        //    MemoryStream ms = new MemoryStream(data);
        //    using (BsonReader reader = new BsonReader(ms))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        return serializer.Deserialize(reader);
        //    }
        //}
        //private static string Decompress(string base64)
        //{
        //    using (var msi = new MemoryStream(Convert.FromBase64String(base64)))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        //        {
        //            gs.CopyTo(mso);
        //        }
        //        return Encoding.UTF8.GetString(mso.ToArray());
        //    }
        //}
        //private static string Compress(string str)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(str);
        //    var hash = MD5.Create().ComputeHash(bytes);

        //    using (var msi = new MemoryStream(bytes))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        //        {
        //            msi.CopyTo(gs);
        //        }

        //        return Convert.ToBase64String(mso.ToArray());
        //    }

        //}

        public static IEnumerable<Type> GetTypes<T>(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                {
                    yield return type;
                }
            }
        }


    }
}