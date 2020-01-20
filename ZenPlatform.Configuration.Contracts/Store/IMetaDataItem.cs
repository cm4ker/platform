namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IMetaDataItem
    {

    }
    public interface IMetaDataItem<T>: IMetaDataItem where T: IMDSettingsItem
    {
        void Initialize(IXCLoader loader, T settings);

        IMDSettingsItem Store(IXCSaver saver);

    }
}
