namespace ZenPlatform.Core.Network.Contracts
{
    public interface IConnectionFactory
    {
        Connection CreateConnection(ITransportClient transportClient);
    }
}