using System;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.AsmClientInfrastructure
{
    /// <summary>
    /// Оснавная точка входа в программу
    /// </summary>
    public class Infrastructure
    {
        public static void Main(IClientInvoker client)
        {
            GlobalScope.Client = client;
        }
    }

    public static class GlobalScope
    {
        private static IClientInvoker _client;

        public static IClientInvoker Client
        {
            get => _client ?? throw new PlatformNotInitializedException();
            set => _client = value;
        }

        
    }

    public class PlatformNotInitializedException : Exception
    {
    }
}