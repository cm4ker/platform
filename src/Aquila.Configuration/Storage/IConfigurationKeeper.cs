using Aquila.Configuration.Contracts;

namespace Aquila.Configuration.Storage
{
    public interface IConfigurationKeeper
    {
        void Save(IXCSaveable data);

        byte[] Load(IXCBlob blob);
    }
}