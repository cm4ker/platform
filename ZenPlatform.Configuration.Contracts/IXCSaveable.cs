using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaveable
    {
        IXCBlob GetBlob();

        byte[] GetData();

        void SetHash(string hash);
    }
}
