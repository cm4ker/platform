using System;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ClientRuntime
{
    /// <summary>
    /// Оснавная точка входа в программу
    /// </summary>
    public class Infrastructure
    {
        public static void Main(IPlatformClient client)
        {
            GlobalScope.Client = client;
        }
    }
    public class PlatformNotInitializedException : Exception
    {
    }
    
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