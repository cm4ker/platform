using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Logging;
using ZenPlatform.ServerClientShared.Logging;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class UserMessageHandler : IUserMessageHandler
    {
        private Action<object, IChannel> _handler;
        private ILogger<UserMessageHandler> _logger;
        private IEnvironmentManager _environmentManager;
        private IUser _user = null;

        public UserMessageHandler(ILogger<UserMessageHandler> logger, IEnvironmentManager environmentManager)
        {
            _logger = logger;
            _environmentManager = environmentManager;
            SetConnectedState();
        }

        public void Receive(object message, IChannel channel)
        {
            
            _handler(message, channel);
        }

        private void  Send(object message, IChannel channel)
        {
            
            channel.Send(message);
        }

        private void SetConnectedState()
        {
            _handler = (message, channel) =>
            {
                switch (message)
                {
                    case RequestEnvironmentListNetworkMessage msg:
                        var responce = new ResponceEnvironmentListNetworkMessage(msg.Id, _environmentManager.GetEnvironmentList());
                        Send(responce, channel);
                        break;
                    case RequestEnvironmentUseNetworkMessage msg:
                        try
                        {
                            SetEnvironmentUsedState(_environmentManager.GetEnvironment(msg.Name));
                            Send(new ResponceEnvironmentUseNetworkMessage(msg), channel);
                        }
                        catch (Exception ex)
                        {
                            Send(new ErrorNetworkMessage(msg.Id, $"Environment {msg.Name} not exist."), channel);
                        }
                        break;
                    default:


                        break;
                }
            };
        }

        private Dictionary<Guid, IMessageHandler> guidHandlers = new Dictionary<Guid, IMessageHandler>();

        private void SetEnvironmentUsedState(IEnvironment env)
        {
            _handler = async (message, channel) =>
            {
                switch (message)
                {
                    case RequestInvokeUnaryNetworkMessage msg:
                        try
                        {
                            Send(new ResponceInvokeUnaryNetworkMessage(msg.Id, await env.InvokeService.Invoke(msg.Route, null, msg.Request)), channel);
                        }
                        catch (Exception ex)
                        {
                            Send(new ErrorNetworkMessage(msg.Id, $"Error invoke method '{msg.Route}'", ex), channel);
                        }
                        break;
                    case StartInvokeStreamNetworkMessage msg:
                        var stream = new DataStream(msg.Id, channel);
                        guidHandlers.Add(msg.Id, stream);
                        await env.InvokeService.InvokeStream(msg.Route, null, stream, msg.Request);
                        break;
                    case DataStreamNetworkMessage msg:
                        if (guidHandlers.TryGetValue(msg.Id,out var dataHandler))
                        {
                            dataHandler.Receive(msg, channel);
                        }
                        break;

                    case EndInvokeStreamNetworkMessage msg:
                        if (guidHandlers.TryGetValue(msg.Id, out var endHandler))
                        {
                            endHandler.Receive(msg, channel);
                            guidHandlers.Remove(msg.Id);
                        }
                        break;
                }
            };


        }

    }
}
