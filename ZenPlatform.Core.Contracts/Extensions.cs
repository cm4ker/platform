using ZenPlatform.Core.Contracts.Network;

namespace ZenPlatform.Core.Contracts
{
    public static class Extensions
    {
        public static TResponce Invoke<TResponce>(this IProtocolClient client, string path, params object[] args)
        {
            return client.Invoke<TResponce>(new Route(path), args);
        }
    }
}