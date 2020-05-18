using Aquila.Core.Contracts.Configuration;

namespace Aquila.Configuration.Storage
{
    public interface IConfigurationKeeper
    {
        void Save(IXCSaveable data);

        byte[] Load(IXCBlob blob);
    }
}