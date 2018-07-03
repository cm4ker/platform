using ZenPlatform.Configuration.ConfigurationLoader.Structure;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Complex;
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
        /// <param name="path">Путь до файла компонента</param>
        /// <returns></returns>
        XCObjectTypeBase LoadObject(string path);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        XCDataRuleBase LoadRule(XCDataRuleContent content);
    }
}