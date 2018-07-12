using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.ConfigurationLoader.Contracts
{
    /// <summary>
    /// Загрузчик компонента
    /// Является мостиком между компонентом и платформой
    /// </summary>
    public interface IXComponentLoader
    {
        /// <summary>
        /// Инициализировать компомент
        /// При вызове этого метода будет инициализирована сущность <see>
        ///         <cref>DataComponentBase</cref>
        ///     </see>
        /// </summary>
        IDataComponent GetComponentImpl(XCComponent component);

        /// <summary>
        /// Загрузить тип компонента
        /// </summary>
        /// <param name="com">Компонент</param>
        /// <param name="blob">Хранилище сериализованного объекта</param>
        /// <returns></returns>
        XCObjectTypeBase LoadObject(XCComponent com, XCBlob blob);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        XCDataRuleBase LoadRule(XCDataRuleContent content);
    }
}