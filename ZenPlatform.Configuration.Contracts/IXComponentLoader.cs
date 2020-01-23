using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Editors;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

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
        IDataComponent GetComponentImpl(IXCComponent component);

        /// <summary>
        /// Загрузить тип компонента
        /// </summary>
        /// <param name="loader">Загрущик из хранилища</param>
        /// <param name="reference">ссылка на объект</param>
        /// <returns></returns>
        void LoadObject(IXCComponent com, IXCLoader loader, string reference);

        /// <summary>
        /// Сохранить обхект
        /// </summary>
        /// <param name="conf"></param>
        void SaveTypeMD(IMDType type, IXCSaver saver);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        IXCDataRule LoadRule(IXCDataRuleContent content);

        /// <summary>
        /// Сохранить правило 
        /// </summary>
        /// <param name="rule"></param>
        void SaveRule(IXCDataRule rule);
    }


    /// <summary>
    /// Менеджер конфигурации компонента. Отвечает за создание и удаление сущности
    /// </summary>
    public interface IXComponentManager
    {
        /// <summary>
        /// Создать новую сущност 
        /// </summary>
        /// <returns>Редактор нового объект конфигурации</returns>
        ITypeEditor Create();

        /// <summary>
        /// Удалить объект конфигурации. Причём конфигурация остаётся в целостном состоянии до и после удаления
        /// </summary>
        /// <param name="type"></param>
        void Delete(IXCObjectType type);
    }
}