using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaveable
    {
        IXCBlob GetBlob();

        byte[] GetData();

        void SetHash(string hash);
    }
}
