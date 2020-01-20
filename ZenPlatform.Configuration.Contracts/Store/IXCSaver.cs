using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaver
    {
        void SaveObject<T>(string path, IMetaDataItem<T> item)
            where
            T : IMDSettingsItem;

        void SaveBytes(string path, byte[] data);
    }
}
