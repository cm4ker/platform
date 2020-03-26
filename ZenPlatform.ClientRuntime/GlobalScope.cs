using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ClientRuntime
{
    public static class GlobalScope
    {
        private static IPlatformClient _client;

        public static IPlatformClient Client
        {
            get => _client ?? throw new PlatformNotInitializedException();
            set => _client = value;
        }
    }
}