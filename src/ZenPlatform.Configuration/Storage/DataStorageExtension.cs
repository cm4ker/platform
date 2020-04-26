using System;

namespace ZenPlatform.Configuration.Storage
{
    public static class DataStorageExtension
    {

        public static void Save(this IDataStorage storage, string uri, byte[] data)
        {
           storage.Save(new Uri(uri), data);
        }

    }
}
