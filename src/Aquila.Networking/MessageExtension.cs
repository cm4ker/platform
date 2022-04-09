using System;
using System.Linq;
using System.Threading.Tasks;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Instance;

namespace Aquila.Core.Network
{
    public static class MessageExtension
    {
        public static async Task<INetworkMessage> Invoke(this RequestInvokeUnaryNetworkMessage request,
            IInvokeService service)
        {
            try
            {
                return new ResponceInvokeUnaryNetworkMessage(request.Id,
                    await service.Invoke(request.Route, request.Args));
            }
            catch (Exception ex)
            {
                return new ErrorNetworkMessage(request.Id, $"Error invoke method '{request.Route}'", ex);
            }
        }

        public static async Task<INetworkMessage> InvokeStream(this StartInvokeStreamNetworkMessage request,
            IInvokeService service, IChannel channel)
        {
            try
            {
                // var stream = new DataStream(request.Id, channel);
                // await service.InvokeStream(request.Route, session, stream, request.Request);
                return new OkNetworkMessage(request.Id);
            }
            catch (Exception ex)
            {
                return new ErrorNetworkMessage(request.Id, $"Error invoke method '{request.Route}'", ex);
            }
        }

        public static async Task<INetworkMessage> Authentication(this RequestAuthenticationNetworkMessage request, Action<AqUser> AuthCallback)
        {
            try
            {
                // var user = manager.Authenticate(request.Token);
                // AuthCallback(user);
                return new ResponceAuthenticationNetworkMessage(request);
            }
            catch (Exception ex)
            {
                return new ErrorNetworkMessage(request.Id, "Authentication error", ex);
            }
        }

        public static async Task<INetworkMessage> GetEnvironmentList(this RequestEnvironmentListNetworkMessage request,
            IAqInstanceManager manager)
        {
            try
            {
                return new ResponceEnvironmentListNetworkMessage(request.Id,
                    manager.GetInstances().Select(env => env.Name).ToList());
            }
            catch (Exception ex)
            {
                return new ErrorNetworkMessage(request.Id, "Get environment list error", ex);
            }
        }

        public static async Task<INetworkMessage> UseEnvironment(this RequestEnvironmentUseNetworkMessage request,
            IAqInstanceManager manager, Action<AqInstance> useCallBack)
        {
            try
            {
                var env = manager.GetInstance(request.Name);
                useCallBack(env);
                return new ResponceEnvironmentUseNetworkMessage(request);
            }
            catch (Exception)
            {
                return new ErrorNetworkMessage(request.Id, $"Environment {request.Name} not exist.");
            }
        }

        public static void PipeTo<T>(this Task<T> task, Guid id, IChannel channel)
        {
            if (!task.IsFaulted || task.IsCompletedSuccessfully)
            {
                channel.Send(task.Result);
            }
            else
            {
                channel.Send(new ErrorNetworkMessage(id, "Unhandled exception.", task.Exception));
            }
        }
    }
}