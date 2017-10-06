using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace CodeGeneration.Configuration
{
    public class CNHash
    {
        private readonly ConfigurationNode _node;

        public CNHash(ConfigurationNode node)
        {
            _node = node;
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

        private string GetHash()
        {
            using (MD5 hash = MD5.Create())
            {
                var data = hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_node)));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public static bool operator ==(CNHash hash1, CNHash hash2)
        {
            if (hash1 is null && hash2 is null) return true;
            if (hash1 is null || hash2 is null) return false;

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hash1.GetHash(), hash2.GetHash()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(CNHash hash1, CNHash hash2)
        {
            return !(hash1 == hash2);
        }
    }
}