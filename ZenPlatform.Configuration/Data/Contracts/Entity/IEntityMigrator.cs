using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

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
        //IList<SqlNode> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual);
//        IExpression GetStep1(XCObjectTypeBase old, XCObjectTypeBase actual);
//
//        IExpression GetStep2(XCObjectTypeBase old, XCObjectTypeBase actual);
//        IExpression GetStep3(XCObjectTypeBase old, XCObjectTypeBase actual);
//        IExpression GetStep4(XCObjectTypeBase old, XCObjectTypeBase actual);
    }
}