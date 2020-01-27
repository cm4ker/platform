using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;


namespace ZenPlatform.Configuration
{
    public class XCMemoryStorage : IXCConfigurationStorage
    {
        private uint _maxId = 100;
        public Dictionary<string, byte[]> Blobs { get; }

        public XCMemoryStorage()
        {
            Blobs = new Dictionary<string, byte[]>();
        }

        public Stream GetBlob(string name, string route)
        {
            
            var path = $"{route}{name}";
            if (Blobs.ContainsKey(path))
            {
                return new MemoryStream(Blobs[path]);
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


            if (Blobs.ContainsKey(path))
            {
                Blobs[path] = buffer;
            } else
            {
                Blobs.Add(path, buffer);
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

        public uint GetId(Guid confId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObjectSetting> GetSettings()
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdateSetting(IObjectSetting setting)
        {
            throw new NotImplementedException();
        }
    }
}