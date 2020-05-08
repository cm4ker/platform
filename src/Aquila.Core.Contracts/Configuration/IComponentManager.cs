using SharpFileSystem;
using Aquila.Configuration.Contracts.Data;
using Aquila.Configuration.Contracts.Store;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure;

namespace Aquila.Configuration.Contracts
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