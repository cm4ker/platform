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
            GlobalScope.Interop = new UXInterop();
        }
    }
}