using System.Collections.Generic;
using System.Runtime.Caching;
using ZenPlatform.Contracts.Entity;

namespace ZenPlatform.Contracts.Data
{
    public interface IDataComponent
    {
        /// <summary>
        /// Событие, вызываемое перед инициализацией компонента
        /// </summary>
        void OnInitializing();

        string Name { get; }
        string Version { get; }

        IEntityManager Manager { get; }
        IEntityGenerator Generator { get; }


    }
}