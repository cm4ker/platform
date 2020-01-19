namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IXCConfigurationItem
    {

    }
    public interface IXCConfigurationItem<T>: IXCConfigurationItem where T: IXCSettingsItem
    {
        void Initialize(IXCLoader loader, T settings);

        IXCSettingsItem Store(IXCSaver saver);

    }
}
