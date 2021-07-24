using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.Network;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.Migrations;

namespace Aquila.Core.Environment
{
    /// <summary>
    /// Рабочая среда. Здесь же реализованы все плюшки  манипуляций с данными и так далее
    /// </summary>
    public class DatabaseTestEnvironment : PlatformEnvironment, IWorkEnvironment
    {
        private object _locking;

        public DatabaseTestEnvironment(IInvokeService invokeService, ILinkFactory linkFactory,
            ILogger<WorkEnvironment> logger,
            IAuthenticationManager authenticationManager, IServiceProvider serviceProvider,
            DataContextManager contextManager, IUserManager userManager, ICacheService cacheService,
            MigrationManager manager
        ) : base(contextManager, cacheService)
        {
            _locking = new object();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _userManager = userManager;

            InvokeService = invokeService;
            LinkFactory = linkFactory;


            MigrationManager = manager;


            Globals = new Dictionary<string, object>();
            AuthenticationManager = authenticationManager;
        }


        public override void Initialize(IStartupConfig config)
        {
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);

            /*
            var newProject = XCRoot.Create("Library");

            
            using (var dataContext = new DataContext(config.DatabaseType, config.ConnectionString))
            {
                var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME,
                    dataContext);

                newProject.Save(configStorage);
            }
            
            */

            //Сначала проинициализируем основные подсистемы платформы, а уже затем рабочую среду
            var savedConfiguration = TestMetadata.GetTestMetadata();
            EntityMetadataCollection currentConfiguration = new EntityMetadataCollection();


            if (MigrationManager.CheckMigration(currentConfiguration, savedConfiguration))
            {
                DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
                MigrationManager.Migrate(currentConfiguration, savedConfiguration);
            }
            //
            // base.Initialize(config);
            // _logger.Info("TEST Database '{0}' loaded.", Configuration.ProjectName);
            //
            // AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
            //
            // if (_assemblyManager.CheckConfiguration(Configuration))
            //     _assemblyManager.BuildConfiguration(Configuration, StartupConfig.DatabaseType);
            //
            // var asms = _assemblyManager.GetAssemblies(Configuration).First(x => x.Type == AssemblyType.Server);
            //
            // var bytes = _assemblyManager.GetAssemblyBytes(asms);
            //
            // _logger.Info("Starting load server assembly");
            // var serverAssembly = Assembly.Load(bytes);
            // var serviceType = serverAssembly.GetType("EntryPoint") ??
            //                   throw new Exception("Entry point in assembly not defined");
            // var main = serviceType.GetMethod("Main") ?? throw new Exception("Main method not found");
            //
            //
            // _logger.Info("Run entry point of server assembly");
            // main.Invoke(null, new[] { new object[] { InvokeService, LinkFactory, _startupService } });
            //
            // InvokeService.Register(new Route("test"), (c, a) => (int)a[0] + 1);
            // InvokeService.RegisterStream(new Route("stream"), (context, stream, arg) =>
            // {
            //     using (StreamWriter writer = new StreamWriter(stream))
            //     {
            //         writer.WriteLine("dsadsdasdasdasdsadasdsadsd");
            //     }
            // });
        }

        private ILogger _logger;

        private IServiceProvider _serviceProvider;

        private IUserManager _userManager;

        public override IInvokeService InvokeService { get; }

        public override IAuthenticationManager AuthenticationManager { get; }

        public MigrationManager MigrationManager { get; }


        /// <summary>
        /// Сборка конфигурации.
        /// В сборке хранятся все типы и бизнес логика
        /// </summary>
        public Assembly Build { get; set; }

        /// <summary>
        /// Глобальные объекты
        /// </summary>
        public Dictionary<string, object> Globals { get; set; }


        /// <summary>
        /// Создаёт сессию для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если платформа не инициализирована</exception>
        public override ISession CreateSession(IUser user)
        {
            lock (_locking)
            {
                //if (!Sessions.Any()) throw new Exception("The environment not initialized!");
                //var id = Sessions.Max(x => x.Id) + 1;
                var session = new UserSession(this, user, DataContextManager, CacheService);
                Sessions.Add(session);

                return session;
            }
        }

        /// <summary>
        /// Убить сессию
        /// </summary>
        /// <param name="session"></param>
        public void KillSession(ISession session)
        {
            lock (_locking)
            {
                Sessions.Remove(session);
            }
        }

        /// <summary>
        /// Убить сессию
        /// </summary>
        /// <param name="id"></param>
        public void KillSession(Guid id)
        {
            lock (_locking)
            {
                var session = Sessions.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Session not found");
                Sessions.Remove(session);
            }
        }

        public ILinkFactory LinkFactory { get; }
    }
}