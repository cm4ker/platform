using SharpFileSystem;
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
        object Load(IProject proj, IComponentRef comRef, IFileSystem fs);

        void Save(IInfrastructure inf, IComponentRef comRef, IFileSystem fs);
    }
}