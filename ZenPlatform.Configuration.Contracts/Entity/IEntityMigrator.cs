using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Configuration.Contracts.Entity
{
    /// <summary>
    /// Интерфейс для получения скриптов обновления и миграции сущностей
    /// </summary>
    public interface IEntityMigrator
    {
        SSyntaxNode GetStep1(IXCObjectType old, IXCObjectType actual);

        SSyntaxNode GetStep2(IXCObjectType old, IXCObjectType actual);
        
        SSyntaxNode GetStep3(IXCObjectType old, IXCObjectType actual);
        
        SSyntaxNode GetStep4(IXCObjectType old, IXCObjectType actual);
    }
}