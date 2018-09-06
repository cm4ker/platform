using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.Configuration.Data.Contracts.Entity
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
        IList<SqlNode> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual);
    }
}