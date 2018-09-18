using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.Data.Contracts
{
    public interface IDataComponent
    {
        /// <summary>
        /// Событие, вызываемое перед инициализацией компонента
        /// </summary>
        void OnInitializing();

        /// <summary>
        /// Менеджер сущностей
        /// </summary>
        IEntityManager Manager { get; }

        /// <summary>
        /// Менеджер компонента. Обеспечивает корректное добавление, удаление объектов и другие операции с конфигурацией, касаемо данного комопнента
        /// </summary>
        IXComponentManager ComponentManager { get; }

        /// <summary>
        /// Генератор сущностей необходимо на стадии сборки проекта
        /// </summary>
        IEntityGenerator Generator { get; }

        /// <summary>
        /// Генератор объектов базы данных
        /// </summary>
        IDatabaseObjectsGenerator DatabaseObjectsGenerator { get; }

        /// <summary>
        /// Мигратор. Позволяет сравнить две сущности одинакового типа и решить, что нужно делать
        /// Миграция происходит после успешной компиляции и готовым блобом с новой сборкой
        /// </summary>
        IEntityMigrator Migrator { get; }


        /// <summary>
        /// Инъектор запросов
        /// </summary>
        IQueryInjector QueryInjector { get; }
    }
}