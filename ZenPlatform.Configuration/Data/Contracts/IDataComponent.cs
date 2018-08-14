using ZenPlatform.Configuration.Data.Contracts.Entity;

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

    }
}