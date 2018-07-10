using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Contracts.Data;

namespace ZenPlatform.Configuration.ConfigurationLoader.Contracts
{
    /// <summary>
    /// Загрузчик конфигурации.
    /// </summary>
    public interface IXCLoader
    {
        /// <summary>
        /// Инициализировать компомент
        /// При вызове этого метода будет инициализирована сущность <see cref="DataComponentBase"/>
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
        /// <param name="xml"></param>
        /// <returns></returns>
        XCDataRuleBase LoadRule(XCDataRuleContent content);
    }
}