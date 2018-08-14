using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ZenPlatform.Configuration
{
    public class XCFileSystemStorage : IXCConfigurationStorage
    {
        private readonly string _directory;
        private readonly string _projectFileName;

        public XCFileSystemStorage(string directory, string projectFileName)
        {
            _directory = directory;
            _projectFileName = projectFileName;
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
            using (var reader = File.OpenRead(Path.Combine(_directory, _projectFileName)))
            {
                var data = new byte[reader.Length];
                reader.Read(data, 0, data.Length);
                return data;
            }
        }

        public string GetStringRootBlob()
        {
            return Encoding.UTF8.GetString(GetRootBlob());
        }

        public void SaveRootBlob(string content)
        {
            SaveBlob(_projectFileName, "", content);
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