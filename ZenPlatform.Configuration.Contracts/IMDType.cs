using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Метаданные типа
    /// </summary>
    public interface IMDType 
    {

    }


    public interface IMDType<T> : IMDType, IXCConfigurationItem<T>
        where T: IXCSettingsItem
    {

    }
    
}