using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaver
    {
        void Save<T>(string path, IXCConfigurationItem<T> item)
            where
            T : IXCSettingsItem;

        void Save(string path, byte[] data);
    }
}
