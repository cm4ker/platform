using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCConfigurationItem
    {

    }
    public interface IXCConfigurationItem<T>: IXCConfigurationItem where T: IXCSettingsItem
    {
        void Initialize(IXCLoader loader, T settings);

        IXCSettingsItem Store(IXCSaver saver);

    }
}
