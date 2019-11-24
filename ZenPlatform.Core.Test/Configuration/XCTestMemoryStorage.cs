using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration;


namespace ZenPlatform.Core.Test.Configuration
{
    public class XCTestMemoryStorage : IXCConfigurationStorage
    {
        private uint _maxId = 100;
        private Dictionary<string, byte[]> _blobs = new Dictionary<string, byte[]>();
        public Stream GetBlob(string name, string route)
        {
            var path = $"{route}{name}";
            if (_blobs.ContainsKey(path))
            {
                return new MemoryStream(_blobs[path]);
            }

            return new MemoryStream();
        }

        public void GetId(Guid confId, ref uint uid)
        {
            if (uid != 0)
                return;
            uid = _maxId++;
        }


        public void SaveBlob(string name, string route, Stream stream)
        {
            var path = $"{route}{name}";
            var buffer = new byte[stream.Length];
            stream.Read(buffer);


            if (_blobs.ContainsKey(path))
            {
                _blobs[path] = buffer;
            } else
            {
                _blobs.Add(path, buffer);
            }

        }

        public Stream GetRootBlob()
        {
            return GetBlob("root", "");
        }

        public void SaveRootBlob(Stream stream)
        {
            SaveBlob("root", "", stream);
        }
    }
}