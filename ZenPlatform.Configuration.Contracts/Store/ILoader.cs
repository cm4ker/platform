using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface ILoader
    {
        T LoadObject<T>(string path);

        byte[] LoadBytes(string path);

        IUniqueCounter Counter { get; }
        ITypeManager TypeManager { get; }
        ISettingsManager Settings { get; }
    }
}