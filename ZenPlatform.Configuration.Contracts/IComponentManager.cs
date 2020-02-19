using SharpFileSystem;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Загрузчик компонента
    /// Является мостиком между компонентом и платформой
    /// </summary>
    public interface IComponentManager
    {
        IComponentEditor Load(IProject proj, IComponentRef comRef, IFileSystem fs);

        void Save(IInfrastructure inf, IComponentRef comRef, IFileSystem fs);
    }
}