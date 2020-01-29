using System;

namespace ZenPlatform.Configuration.Storage
{
    public interface IDataStorage
    {
        Uri Save(Uri uri, byte[] data);

        byte[] Load(Uri uri);

        bool CanUse(Uri uri);
    }
}