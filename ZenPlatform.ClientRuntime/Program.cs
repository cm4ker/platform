using System;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.ClientRuntime
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

  

}