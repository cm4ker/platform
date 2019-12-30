using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog.LayoutRenderers;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
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
        private IStartupConfig _config;
        private ILogger _logger;
        private IAssemblyManager _assemblyManager;

        public IList<ISession> Sessions { get; }

        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public string Name => "Library";

        public IDataContextManager DataContextManager => throw new NotImplementedException();

        public IXCRoot Configuration => Factory.CreateExampleConfiguration();

        public TestEnvironment(IAuthenticationManager authenticationManager, IInvokeService invokeService,
            ILogger<TestEnvironment> logger,
            IAssemblyManager assemblyManager)
        {
            Sessions = new RemovingList<ISession>();
            AuthenticationManager = authenticationManager;
            AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
            InvokeService = invokeService;
            _assemblyManager = assemblyManager;
            _logger = logger;
        }

        public ISession CreateSession(IUser user)
        {
            var session = new SimpleSession(this, user);
            return session;
        }

        public void Initialize(IStartupConfig config)
        {
            _config = config;
            _logger.Info("TEST ENVIRONMENT START.");


            if (_assemblyManager.CheckConfiguration(Configuration))
                _assemblyManager.BuildConfiguration(Configuration, _config.DatabaseType);


            var asms = _assemblyManager.GetAssemblies(Configuration).First(x => x.Type == AssemblyType.Server);

            var bytes = _assemblyManager.GetAssemblyBytes(asms);
            var serverAssembly = Assembly.Load(bytes);

            var serviceType = serverAssembly.GetType("Service.ServerInitializer");
            var initializerInstance = (IServerInitializer) Activator.CreateInstance(serviceType, InvokeService);
            initializerInstance.Init();

            InvokeService.Register(new Route("test"), (c, a) => { return (int) a[0] + 1; });

            InvokeService.Register(new Route("Test_GetInvoice"),
                (c, a) =>
                {
                    return new ViewBag {{"Id", Guid.Parse("8b888935-895d-4806-beaf-0f9e9217ad1b")}, {"Type", 10}};
                });

            InvokeService.Register(new Route("Test_GetProperty"),
                (c, a) =>
                {
                    var typeId = (int) a[0];
                    var propName = (string) a[1];
                    var id = (Guid) a[2];

                    if (typeId == 10 && propName == "Store" && id == Guid.Parse("8b888935-895d-4806-beaf-0f9e9217ad1b"))
                    {
                        return new ViewBag {{"Id", Guid.Parse("9de86d2e-1597-4518-b24c-8bfe7f25bf50")}, {"Type", 11}};
                    }

                    return new ViewBag {{"Id", Guid.Empty}, {"Type", 11}};
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