namespace Aquila.Core.Contracts.Configuration
{
    public interface IXCSaveable
    {
        IXCBlob GetBlob();

        byte[] GetData();

        void SetHash(string hash);
    }
}
