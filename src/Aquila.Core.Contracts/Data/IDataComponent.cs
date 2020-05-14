using Aquila.Core.Contracts.Data.Entity;

namespace Aquila.Core.Contracts.Data
{
    /// <summary>
    /// Интерфейс комопнента данных
    /// </summary>
    public interface IDataComponent : IMigrateable, IInternalQueryParticipant, IBuildingParticipant
    {
    }


    public interface IBuildingParticipant
    {
        IPlatformGenerator Generator { get; }
    }

    public interface IMigrateable
    {
        /// <summary>
        /// Мигратор. Позволяет сравнить две сущности одинакового типа и решить, что нужно делать
        /// Миграция происходит после успешной компиляции и готовым блобом с новой сборкой
        /// </summary>
        IEntityMigrator Migrator { get; }
    }

    public interface IInternalQueryParticipant
    {
        /// <summary>
        /// Инъектор запросов
        /// </summary>
        IQueryInjector QueryInjector { get; }
    }
}