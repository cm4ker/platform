

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
    public interface IComponentLoader
    {
 
        /// <summary>
        /// Загрузить тип компонента
        /// </summary>
        /// <param name="loader">Загрущик из хранилища</param>
        /// <param name="reference">ссылка на объект</param>
        /// <returns></returns>
        void LoadObject(IComponent com, ILoader loader, string reference);

        IDataComponent GetComponentImpl(IComponent c);
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