using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaver
    {
        void SaveObject<T>(string path, IXCConfigurationItem<T> item)
            where
            T : IXCSettingsItem;

        void SaveBytes(string path, byte[] data);
    }
}
