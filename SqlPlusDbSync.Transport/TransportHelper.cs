using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace SqlPlusDbSync.Transport
{
    public static class TransportHelper
    {
        public static string Pack(object obj)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return Compress(JsonConvert.SerializeObject(obj, settings));
        }

        public static byte[] PackToArray(object obj)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return CompressToArray(JsonConvert.SerializeObject(obj, settings));
        }

        public static T Unpack<T>(string str)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return (T)JsonConvert.DeserializeObject(Decompress(str), settings);
        }

        public static T UnpackArray<T>(byte[] data)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return (T)JsonConvert.DeserializeObject(DecompressArray(data), settings);
        }


        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            if (b1 is null || b2 is null) return false;
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        public static byte[] SerializeBson(object o)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, o);
            }
            return ms.ToArray();
        }

        public static T DeserializeBson<T>(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            using (BsonReader reader = new BsonReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();

                return serializer.Deserialize<T>(reader);
            }
        }

        private static string Decompress(string base64)
        {
            using (var msi = new MemoryStream(Convert.FromBase64String(base64)))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        private static string Compress(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var hash = MD5.Create().ComputeHash(bytes);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }

        }

        private static string DecompressArray(byte[] data)
        {
            using (var msi = new MemoryStream(data))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
        private static byte[] CompressToArray(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var hash = MD5.Create().ComputeHash(bytes);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }
    }
}