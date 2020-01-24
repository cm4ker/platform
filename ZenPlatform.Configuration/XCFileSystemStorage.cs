using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration
{
    public class XCFileSystemStorage : IXCConfigurationStorage, IDisposable
    {
        private readonly string _directory;
        private readonly string _projectFileName;
        private uint _maxId = 100;
        private List<IDisposable> _disposables = new List<IDisposable>();

        public XCFileSystemStorage(string directory, string projectFileName)
        {
            _directory = directory;
            _projectFileName = projectFileName;
        }

        public Stream GetBlob(string name, string route)
        {
            var stream = File.OpenRead(Path.Combine(_directory, route, name));
            _disposables.Add(stream);
            return stream;
        }

        public Stream GetRootBlob()
        {
            var stream = File.OpenRead(Path.Combine(_directory, _projectFileName));
            _disposables.Add(stream);
            return stream;
        }

        public void SaveRootBlob(Stream stream)
        {
            SaveBlob(_projectFileName, "", stream);
        }

        public void GetId(Guid confId, ref uint uid)
        {
            if (uid != 0)
                return;

            uid = _maxId++;
        }

        public void SaveBlob(string name, string route, Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var tmp = route + name;
           // var folder = Pathname.Combine(_directory, route);
           if (!Directory.Exists(_directory))
            Directory.CreateDirectory(_directory);

            using (var sw = File.Open(Path.Combine(_directory, tmp), FileMode.Create))
            {
                stream.CopyTo(sw);
            }
        }

        public void Dispose()
        {
            foreach (var d in _disposables)
            {
                d.Dispose();
            }
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