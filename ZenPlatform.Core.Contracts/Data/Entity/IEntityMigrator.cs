using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Migration;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Configuration.Contracts.Data.Entity

{
    /// <summary>
    /// Интерфейс для получения скриптов обновления и миграции сущностей
    /// </summary>
    public interface IEntityMigrator
    {
        /// <summary>
        /// Поулчить скрипт мигрирования по объекту
        /// </summary>
        /// <param name="old">Старый объект</param>
        /// <param name="actual">Текущий объект</param>
        /// <returns></returns>
        //IList<SqlNode> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual);
        void MigrationPlan(IEntityMigrationPlan plan, IComponent oldState, IComponent actualState);
    }
}