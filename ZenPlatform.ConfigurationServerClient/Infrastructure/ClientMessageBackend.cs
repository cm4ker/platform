using System;
using System.Linq;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.IdeIntegration.Shared.Infrastructure;
using ZenPlatform.IdeIntegration.Shared.Messages;
using ZenPlatform.IdeIntegration.Shared.Models;

namespace ZenPlatform.IdeIntegration.Client.Infrastructure
{
    /// <summary>
    /// Обработчик сообщений на клиенте
    /// </summary>
    public class ClientMessageBackend : MessageHandlerBackend
    {
        public ClientMessageBackend()
        {
        }
    }
}