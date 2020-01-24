using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface ILoader
    {
        T LoadObject<T, C>(string path, bool loadTree = true)
            where
            T : class, IMetaDataItem<C>, new()
            where
            C : IMDItem;


        byte[] LoadBytes(string path);

        IUniqueCounter Counter { get; }
        ITypeManager TypeManager { get; }
    }
}