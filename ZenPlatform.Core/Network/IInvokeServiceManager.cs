using System.Threading.Tasks;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network
{
    public interface IInvokeServiceManager
    {
        IInvokeService GetInvokeService(string serviceName);
        Task<ResponceInvokeUnaryNetworkMessage> Invoke(RequestInvokeUnaryNetworkMessage message);
    }
}