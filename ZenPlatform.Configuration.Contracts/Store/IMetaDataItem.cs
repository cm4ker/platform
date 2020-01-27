namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IMetaDataItem
    {

    }
    public interface IMetaDataItem<T>: IMetaDataItem where T: IMDItem
    {
        void OnLoad(ILoader loader, T settings);

        IMDItem OnStore(IXCSaver saver);

    }
}
