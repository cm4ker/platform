using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace RabbitMQClient
{
    public static class TransportHelper
    {
        public static string PackToString(object obj)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return Compress(JsonConvert.SerializeObject(obj, settings));
        }

        public static byte[] PackToArray(object obj)
        {
            return CompressToArray(SerializeBson(obj));
        }

        public static T UnpackString<T>(string str)
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<T>(Decompress(str), settings);
        }

        public static T UnpackArray<T>(byte[] data)
        {
            return DeserializeBson<T>(DecompressArrayToArray(data));
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
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.Serialize(writer, o);
                }
                var result = ms.ToArray();
                Debug.WriteLine($"Serialized object size: {result.Length} bytes ({result.Length / 1024 / 1024}MB)");
                return result;
            }
        }

        public static T DeserializeBson<T>(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            using (BsonReader reader = new BsonReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.All;
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

        private static string Compress(byte[] bytes)
        {
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


        private static byte[] DecompressArrayToArray(byte[] data)
        {
            using (var msi = new MemoryStream(data))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return mso.ToArray();
            }
        }

        private static byte[] CompressToArray(byte[] data)
        {
            using (var msi = new MemoryStream(data))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                var result = mso.ToArray();
                Debug.WriteLine($"Compressed data size: {result.Length} bytes ({result.Length / 1024 / 1024}MB)");
                return result;
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