using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts;
using Aquila.Core.Crypto;

namespace Aquila.Configuration.Storage
{
    public class ConfigurationKeeper: IConfigurationKeeper
    {
        private List<IDataStorage> _storages;
        private IDataStorage _defaultStorage;
        public ConfigurationKeeper(IDataStorage defaultStorage, params IDataStorage[] storages)
        {

            _storages = new List<IDataStorage>();
            _defaultStorage = defaultStorage;
            _storages.Add(defaultStorage);
            if (storages != null)
            {
                _storages.AddRange(storages);
            } 
        }
        public void Save(IXCSaveable data)
        {

            if (_defaultStorage.CanUse(data.GetBlob().URI))
            {
                _defaultStorage.Save(data.GetBlob().URI, data.GetData());
                data.SetHash(HashHelper.HashMD5(data.GetData()));
            }

        }

        public byte[] Load(IXCBlob blob)
        {
            return _storages.First(s => s.CanUse(blob.URI)).Load(blob.URI);
        }
    }
}