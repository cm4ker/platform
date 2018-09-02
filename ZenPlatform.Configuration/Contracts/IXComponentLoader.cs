using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.Contracts
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
        /// Сохранить обхект
        /// </summary>
        /// <param name="conf"></param>
        void SaveObject(XCObjectTypeBase conf);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        XCDataRuleBase LoadRule(XCDataRuleContent content);

        /// <summary>
        /// Сохранить правило 
        /// </summary>
        /// <param name="rule"></param>
        void SaveRule(XCDataRuleBase rule);
    }


    /// <summary>
    /// Менеджер конфигурации компонента. Отвечает за создание и удаление сущности
    /// </summary>
    public interface IXComponentManager
    {
        /// <summary>
        /// Создать новую сущност
        /// </summary>
        /// <param name="parent">Родитель этой сущности</param>
        /// <returns>Новый объект конфигурации</returns>
        XCObjectTypeBase Create(XCObjectTypeBase parentType = null);

        /// <summary>
        /// Удалить объект конфигурации. Причём конфигурация остаётся в целостном состоянии до и после удаления
        /// </summary>
        /// <param name="type"></param>
        void Delete(XCObjectTypeBase type);
    }
}