using Aquila.Ide.Contracts;

namespace Aquila.Core.Contracts.Configuration
{
    public interface IComponentEditor
    {
        IConfigurationItem GetConfigurationTree();
    }
}