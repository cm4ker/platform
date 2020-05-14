using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Crypto;

namespace Aquila.Configuration.Storage
{
    public class MemoryConfigurationKeeper : IConfigurationKeeper
    {
        public MemoryDataStorage _storage = new MemoryDataStorage();
        public byte[] Load(IXCBlob blob)
        {
            return _storage.Load(blob.URI);
        }

        public void Save(IXCSaveable data)
        {
            _storage.Save(data.GetBlob().URI, data.GetData());

            data.SetHash(HashHelper.HashMD5(data.GetData()));
        }
    }
}