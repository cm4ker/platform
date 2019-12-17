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
        SSyntaxNode GetStep1(IXCObjectType old, IXCObjectType actual, DDLQuery query);

        SSyntaxNode GetStep2(IXCObjectType old, IXCObjectType actual, DDLQuery query);
        SSyntaxNode GetStep3(IXCObjectType old, IXCObjectType actual, DDLQuery query);
        SSyntaxNode GetStep4(IXCObjectType old, IXCObjectType actual, DDLQuery query);
    }
}