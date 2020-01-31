using SharpFileSystem;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IInfrastructure
    {
        IUniqueCounter Counter { get; }
        ITypeManager TypeManager { get; }
        ISettingsManager Settings { get; }
        IFileSystem FileSystem { get; }
    }
}