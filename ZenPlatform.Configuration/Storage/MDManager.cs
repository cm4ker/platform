using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Storage
{
    public class MDManager : ILoader, IXCSaver
    {
        private IXCConfigurationStorage _storage;
        private ITypeManager _typeManager;

        public MDManager(IXCConfigurationStorage storage, ITypeManager typeManager)
        {
            _storage = storage;
            _typeManager = typeManager;
        }


        public IXCConfigurationStorage Storage => _storage;

        public IUniqueCounter Counter => _storage;
        public ITypeManager TypeManager => _typeManager;

        public T LoadObject<T, C>(string path, bool loadTree = true)
            where
            T : class, IMetaDataItem<C>, new()
            where
            C : IMDItem
        {
            try
            {
                using (var stream = _storage.GetBlob("", path))
                {
                    var config = XCHelper.DeserializeFromStream<C>(stream);

                    T result;

                    if (config.GetType() == typeof(T))
                        result = config as T;
                    else
                        result = new T();

                    if (result == null)
                        throw new Exception("The result is null");

                    if (loadTree)
                        result.Initialize(this, config);

                    return result;
                }
            }
            catch
            {
                return default;
            }
        }

        public byte[] LoadBytes(string path)
        {
            using (var stream = _storage.GetBlob("", path))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                byte[] myBynary = memoryStream.ToArray();
                return memoryStream.ToArray();
            }
        }

        public void SaveObject<T>(string path, IMetaDataItem<T> item)
            where
            T : IMDItem
        {
            var config = item.Store(this);

            _storage.SaveBlob("", path, config.SerializeToStream());
        }

        public void SaveBytes(string path, byte[] data)
        {
            _storage.SaveBlob("", path, new MemoryStream(data));
        }
    }
}