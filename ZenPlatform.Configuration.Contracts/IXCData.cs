using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public interface IXCData : IChildItem<IXCRoot>
    {
        ChildItemCollection<IXCData, IXCComponent> Components { get; }

        /// <summary>
        /// Все типы платформы
        /// </summary>
        IEnumerable<IXCType> PlatformTypes { get; }

        /// <summary>
        /// Все типы, которые относятся к компонентам
        /// </summary>
        IEnumerable<IXCObjectType> ComponentTypes { get; }

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
        void RegisterType(IXCObjectType type);

        IXCComponent GetComponentByName(string name);
    }
}