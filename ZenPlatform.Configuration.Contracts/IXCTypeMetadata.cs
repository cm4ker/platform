using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Метаданные типа
    /// </summary>
    public interface IXCTypeMetadata 
    {

    }


    public interface IXCTypeMetadata<T> : IXCTypeMetadata, IXCConfigurationItem<T>
        where T: IXCSettingsItem
    {

    }
    
}