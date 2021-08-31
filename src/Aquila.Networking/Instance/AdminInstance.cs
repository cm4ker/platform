using System.Collections.Generic;
using Aquila.Core.Authentication;
using Aquila.Core.Tools;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Sessions;
using Aquila.Logging;

namespace Aquila.Core.Instance
{
    /// <inheritdoc cref="IAdminInstance"/>
    public class AdminInstance : IAdminInstance
    {
        private ILogger _logger;


        public AdminInstance(IAuthenticationManager authenticationManager, IInvokeService invokeService,
            ILogger<AdminInstance> logger)
        {
            Sessions = new RemovingList<ISession>();
            AuthenticationManager = authenticationManager;
            AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
            InvokeService = invokeService;
            _logger = logger;
        }

        public string Name => "admin";

        public IList<ISession> Sessions { get; }

        public IInvokeService InvokeService { get; }

        public void Initialize(object config)
        {
            _logger.Info("Start admin environment.");
        }

        public IAuthenticationManager AuthenticationManager { get; }

        public ISession CreateSession(IUser user)
        {
            var session = new SimpleSession(this, user);
            return session;
        }
    }
}