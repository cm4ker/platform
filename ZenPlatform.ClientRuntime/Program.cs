using System;
using ZenPlatform.Core.Network;

namespace ZenPlatform.AsmClientInfrastructure
{
    /// <summary>
    /// Оснавная точка входа в программу
    /// </summary>
    public class Infrastructure
    {
        public static void Main(Client client)
        {
            GlobalScope.Client = client;
        }
    }

    public static class GlobalScope
    {
        private static Client _client;

        public static Client Client
        {
            get => _client ?? throw new PlatformNotInitializedException();
            set => _client = value;
        }
    }

    public class PlatformNotInitializedException : Exception
    {
    }
}