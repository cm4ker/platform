using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;


namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Загрузчик компонента
    /// Является мостиком между компонентом и платформой
    /// </summary>
    public interface IComponentManager
    {
        void Load(IComponentRef comRef, ILoader loader);

        void Save(IComponentRef comRef, IXCSaver saver);

        IDataComponent GetComponentImpl(IComponent c);
    }
}