namespace ZenPlatform.Configuration.Contracts.Store
{
    public interface IXCLoader
    {
        T LoadObject<T, C>(string path, bool loadTree = true)
            where
            T : class, IMetaDataItem<C>, new()
            where
            C : IMDItem;


        byte[] LoadBytes(string path);
    }



    

}
