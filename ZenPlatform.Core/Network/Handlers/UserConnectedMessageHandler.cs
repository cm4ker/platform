using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Environment;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Core.Network.Handlers
{
    public class UserConnectedMessageHandler : IUserMessageHandler
    {
        public IEnvironmentManager _environmentManager;
        public UserConnectedMessageHandler(IEnvironmentManager environmentManager)
        {
            _environmentManager = environmentManager;
        }

        public void Receive(object message, IChannel channel)
        {
            switch (message)
            {
                case RequestEnvironmentListNetworkMessage msg:
                    var responce = new ResponceEnvironmentListNetworkMessage(msg.Id, _environmentManager.GetEnvironmentList());
                    channel.Send(responce);
                    break;
                case RequestEnvironmentUseNetworkMessage msg:
                    try
                    {
                        channel.SetHandler(new EnvironmentUsedMessageHandler(_environmentManager.GetEnvironment(msg.Name)));
                        channel.Send(new ResponceEnvironmentUseNetworkMessage(msg));
                    }
                    catch (Exception ex)
                    {
                        channel.Send(new ErrorNetworkMessage(msg.Id, $"Environment {msg.Name} not exist."));
                    }
                    break;
                default:


                    break;
            }
        }
    }
}
