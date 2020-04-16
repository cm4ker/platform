using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.Configuration.Structure
{
    public interface IComponentEditor
    {
        IConfigurationItem GetConfigurationTree();
    }
}