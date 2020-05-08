using Aquila.Ide.Contracts;

namespace Aquila.Configuration.Structure
{
    public interface IComponentEditor
    {
        IConfigurationItem GetConfigurationTree();
    }
}