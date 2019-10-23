using System;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Network;

namespace ZenPlatform.ServerRuntime
{
    public class ServerInitializer : IServerInitializer
    {
        private readonly IInvokeService _invokeService;

        public ServerInitializer(IInvokeService invokeService)
        {
            _invokeService = invokeService;
        }

        public void Init()
        {
            _invokeService.Register();
        }
    }
}