using System;
using ZenPlatform.Data;

namespace ZenPlatform.Configuration.Storage
{
    public class DatabaseDataStorage : IDataStorage
    {
        private IDataContextManager _dataContextManager;

        public DatabaseDataStorage(IDataContextManager dataContextManager)
        {
            _dataContextManager = dataContextManager;
        }

        public bool CanUse(Uri uri)
        {
            return string.IsNullOrEmpty(uri.Scheme)
                   && uri.Scheme == "db";
        }

        public byte[] Load(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Uri Save(Uri uri, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}