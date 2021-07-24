using System.Collections.Generic;
using Aquila.Core.Authentication;
using Aquila.Core.Tools;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Sessions;
using Aquila.Logging;

namespace Aquila.Core.Environment
{
    /// <inheritdoc cref="IAdminEnvironment"/>
    public class AdminEnvironment : IAdminEnvironment
    {
        private ILogger _logger;


        public AdminEnvironment(IAuthenticationManager authenticationManager, IInvokeService invokeService,
            ILogger<AdminEnvironment> logger)
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