using System.IO;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration.Storage
{
    public class MDManager : IXCLoader, IXCSaver
    {
        public MDManager(IXCConfigurationStorage storage)
        {
            _storage = storage;
        }

        public IXCConfigurationStorage _storage;

        public T LoadObject<T, C>(string path, bool loadTree = true)
            where
            T : IMetaDataItem<C>, new()
            where
            C : IMDSettingsItem
        {
            try
            {
                using (var stream = _storage.GetBlob("", path))
                {
                    var config = XCHelper.DeserializeFromStream<C>(stream);


                    var result = new T();

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
            T : IMDSettingsItem
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