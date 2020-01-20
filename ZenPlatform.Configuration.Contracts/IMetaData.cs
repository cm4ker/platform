using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Метаданные типа
    /// </summary>
    public interface IMetaData 
    {

    }


    public interface IMetaData<T> : IMetaData, IMetaDataItem<T>
        where T: IMDSettingsItem
    {

    }
    
}