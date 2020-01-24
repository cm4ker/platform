namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IMetaDataItem
    {

    }
    public interface IMetaDataItem<T>: IMetaDataItem where T: IMDItem
    {
        void Initialize(ILoader loader, T settings);

        IMDItem Store(IXCSaver saver);

    }
}
