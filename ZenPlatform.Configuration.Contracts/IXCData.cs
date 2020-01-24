using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCData : IChildItem<IXCRoot>
    {
        ChildItemCollection<IXCData, IComponent> Components { get; }

        /// <summary>
        /// Все типы платформы
        /// </summary>
        IEnumerable<IXCType> PlatformTypes { get; }

        /// <summary>
        /// Все типы, которые относятся к компонентам
        /// </summary>
        IEnumerable<IXCStructureType> StructureTypes { get; }

        /// <summary>
        /// Загрузить дерективу и все зависимости
        /// </summary>
        void Load();

        /// <summary>
        /// Созрание всех данных из раздела Data
        /// </summary>
        void Save();

        /// <summary>
        /// Зарегистрировать тип данных на уровне конфигурации платформы
        /// </summary>
        /// <param name="type"></param>
        void RegisterType(IXCType type);

        IComponent GetComponentByName(string name);
    }
}