using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCLoader
    {
        T Load<T, C>(string path, bool loadTree = true)
            where
            T : IXCConfigurationItem<C>, new()
            where
            C : IXCSettingsItem;


        byte[] Load(string path);
    }



    

}
