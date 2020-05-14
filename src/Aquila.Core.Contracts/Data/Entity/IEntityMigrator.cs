﻿using Aquila.Core.Contracts.Configuration.Migration;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Core.Contracts.Data.Entity

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