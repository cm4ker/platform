using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Crypto;

namespace ZenPlatform.Configuration.CompareTypes
{
    public class XCObjectTypeComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {

            var a = new StreamReader(x.SerializeToStream()).ReadToEnd();
            var b = new StreamReader(y.SerializeToStream()).ReadToEnd();

            return HashHelper.HashMD5(y.SerializeToStream()) == HashHelper.HashMD5(x.SerializeToStream());
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
