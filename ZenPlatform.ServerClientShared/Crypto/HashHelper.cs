using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;
using System.IO;

namespace ZenPlatform.Core.Crypto
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
    }
}
