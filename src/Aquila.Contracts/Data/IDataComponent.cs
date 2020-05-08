using System.Collections.Generic;
using System.Runtime.Caching;
using Aquila.Contracts.Entity;

namespace Aquila.Contracts.Data
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