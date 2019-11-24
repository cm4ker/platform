using System.Collections.Generic;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSessionSetting : IChildItem<IXCRoot>
    {

        /// <summary>
        /// Имя параметра сессии
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Тип параметра сессии
        /// </summary>
        List<IXCType> Types { get; }
    }
}