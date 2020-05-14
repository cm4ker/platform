using System;
using System.Linq;
using Aquila.Configuration;
using Aquila.Configuration.Structure;
using Aquila.IdeIntegration.Shared.Infrastructure;
using Aquila.IdeIntegration.Shared.Messages;
using Aquila.IdeIntegration.Shared.Models;

namespace Aquila.IdeIntegration.Client.Infrastructure
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