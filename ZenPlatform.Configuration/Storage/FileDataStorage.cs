using System;

namespace ZenPlatform.Configuration.Storage
{
    public class FileDataStorage : IDataStorage
    {
        public bool CanUse(Uri uri)
        {
            return string.IsNullOrEmpty(uri.Scheme)
                   && uri.Scheme == "file";
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