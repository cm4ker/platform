using System;
using System.IO;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Storage
{
    public class MDManager : ILoader, IXCSaver
    {
        private IFileSystem _storage;
        private ITypeManager _typeManager;

        public MDManager(IFileSystem storage, ITypeManager typeManager)
        {
            _storage = storage;
            _typeManager = typeManager;
        }


        public IFileSystem Storage => _storage;

        public IUniqueCounter Counter => null;

        public ISettingsManager Settings => null;

        public ITypeManager TypeManager => _typeManager;

        public T LoadObject<T>(string path)
        {
            try
            {
                using (var stream = _storage.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                {
                    return XCHelper.DeserializeFromStream<T>(stream);
                }
            }
            catch
            {
                return default;
            }
        }

        public byte[] LoadBytes(string path)
        {
            using (var stream = _storage.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                byte[] myBynary = memoryStream.ToArray();
                return memoryStream.ToArray();
            }
        }

        public void SaveObject(string path, object obj)
        {
            using (var stream = _storage.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                obj.SerializeToStream().CopyTo(stream);
        }

        public void SaveBytes(string path, byte[] data)
        {
            using (var stream = _storage.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                stream.Write(data, 0, data.Length);
        }
    }
}