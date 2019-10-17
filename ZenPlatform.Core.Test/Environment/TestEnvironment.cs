using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Environment.Contracts;

namespace ZenPlatform.Core.Test.Environment
{
    public class TestEnvironment : IWorkEnvironment
    {
        private StartupConfig _config;
        private ILogger _logger;
        private IAssemblyManager _assemblyManager;

        public IList<ISession> Sessions { get; }

        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public string Name => "Library";

        public IDataContextManager DataContextManager => throw new NotImplementedException();

        public XCRoot Configuration => Tests.Common.Factory.CreateExampleConfiguration();

        public TestEnvironment(IAuthenticationManager authenticationManager, IInvokeService invokeService, ILogger<TestEnvironment> logger,
            IAssemblyManager assemblyManager)
        {
            Sessions = new RemovingList<ISession>();
            AuthenticationManager = authenticationManager;
            AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
            InvokeService = invokeService;
            _assemblyManager = assemblyManager;
            _assemblyManager.CheckConfiguration(Configuration);
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
            _logger.Info("TEST ENVIRONMENT START.");

           


            InvokeService.Register(new Route("test"), (c, a) =>
            {
            return (int)a[0] + 1;
            });


            InvokeService.RegisterStream(new Route("stream"), (context, stream, arg) =>
            {


                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("dsadsdasdasdasdsadasdsadsd");

                }
            });


        }
    }
}
