namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IXCLoader
    {
        T LoadObject<T, C>(string path, bool loadTree = true)
            where
            T : IMetaDataItem<C>, new()
            where
            C : IMDSettingsItem;


        byte[] LoadBytes(string path);
    }



    

}
