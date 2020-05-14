using System;
using Aquila.Configuration.Contracts.Data.Entity;

namespace Aquila.Configuration.Contracts.Data
{
    /// <summary>
    /// Интерфейс комопнента данных
    /// </summary>
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
        //IEntityGenerator Generator { get; }

        IPlatformGenerator Generator { get; }

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