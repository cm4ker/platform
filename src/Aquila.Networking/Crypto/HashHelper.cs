using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Aquila.Core.Crypto
{
    public static class HashHelper
    {
        public static string HashMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var h = md5.ComputeHash(stream);
                return BitConverter.ToString(h).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string HashMD5(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var h = md5.ComputeHash(data);
                return BitConverter.ToString(h).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}