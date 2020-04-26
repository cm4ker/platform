using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Storage
{
    public interface IConfigurationKeeper
    {
        void Save(IXCSaveable data);

        byte[] Load(IXCBlob blob);
    }
}