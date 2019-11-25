using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;


namespace ZenPlatform.Core.Test.Configuration
{
    public class XCTestStorage : IXCConfigurationStorage
    {
        public Stream GetBlob(string name, string route)
        {
            throw new NotImplementedException();
        }

        public void GetId(Guid confId, ref uint oldId)
        {
            throw new NotImplementedException();
        }

        public Stream GetRootBlob()
        {
            throw new NotImplementedException();
        }

        public void SaveBlob(string name, string route, Stream stream)
        {
            throw new NotImplementedException();
        }

        public void SaveRootBlob(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}