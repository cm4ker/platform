using Aquila.Core.Contracts.Configuration.Store;
using SharpFileSystem;

namespace Aquila.Core.Contracts.Configuration
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