using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml.Linq;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.Configuration
{

    public class XCFileSystemStorage : IXCConfigurationStorage
    {
        private readonly string _directory;

        public XCFileSystemStorage(string directory, string fileName)
        {
            _directory = directory;
        }

        public byte[] GetBlob(string name, string route)
        {
            using (var reader = File.OpenRead(Path.Combine(_directory, route, name)))
            {
                var data = new byte[reader.Length];
                reader.Read(data, 0, data.Length);
                return data;
            }
        }

        public string GetStringBlob(string name, string route)
        {
            return Encoding.UTF8.GetString(GetBlob(name, route));
        }

        public void SaveBlob(string name, string route, string data)
        {
            SaveBlob(name, route, Encoding.UTF8.GetBytes(data));
        }

        public byte[] GetRootBlob()
        {
            throw new NotImplementedException();
        }

        public string GetStringRootBlob()
        {
            throw new NotImplementedException();
        }

        public void SaveBlob(string name, string route, byte[] bytes)
        {
            using (var sw = File.OpenWrite(Path.Combine(_directory, route, name)))
            {
                sw.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
