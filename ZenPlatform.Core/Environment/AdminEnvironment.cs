using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Logging;
using System.IO;

namespace ZenPlatform.Core.Environment
{
    public class AdminEnvironment : IEnvironment, IAdminEnvironment
    {
        private StartupConfig _config;
        private ILogger _logger;

        public IList<ISession> Sessions { get; }

        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public string Name => "admin";

        public IDataContextManager DataContextManager => throw new NotImplementedException();

        public AdminEnvironment(IAuthenticationManager authenticationManager, IInvokeService invokeService, ILogger<TestEnvironment> logger)
        {
            Sessions = new RemovingList<ISession>();
            AuthenticationManager = authenticationManager;
            AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
            InvokeService = invokeService;
            _logger = logger;

        }

        public ISession CreateSession(IUser user)
        {
            var session = new SimpleSession(this, user);
            return session;
        }

        public void Initialize(StartupConfig config)
        {
            _config = config;
            _logger.Info("Start admin environment.");
            
        }
    }
}
