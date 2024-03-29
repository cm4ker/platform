﻿namespace Aquila.Core.Instance
{
//     /// <summary>
//     /// Рабочая среда. Здесь же реализованы все плюшки  манипуляций с данными и так далее
//     /// </summary>
//     public class DatabaseTestEnvironment : PlatformEnvironment, IWorkEnvironment
//     {
//         private object _locking;
//
//         public IMigrationManager MigrationManager { get; }
//
//         public DatabaseTestEnvironment(IInvokeService invokeService, ILinkFactory linkFactory,
//             ILogger<WorkEnvironment> logger,
//             IAuthenticationManager authenticationManager, IServiceProvider serviceProvider,
//             IDataContextManager contextManager, IUserManager userManager, ICacheService cacheService,
//             IAssemblyManager assemblyManager, IMigrationManager migrationManager, IStartupService startupService,
//             IConfigurationManipulator manipulator) :
//             base(contextManager, cacheService, manipulator)
//         {
//             _locking = new object();
//             _serviceProvider = serviceProvider;
//             _logger = logger;
//             _userManager = userManager;
//
//             InvokeService = invokeService;
//             LinkFactory = linkFactory;
//
//             _assemblyManager = assemblyManager;
//             _startupService = startupService;
//
//             Globals = new Dictionary<string, object>();
//
//             Managers = new Dictionary<Type, IEntityManager>();
//
//
//             AuthenticationManager = authenticationManager;
//             MigrationManager = migrationManager;
//         }
//
//
//         public override void Initialize(IStartupConfig config)
//         {
//             MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);
//
//             /*
//             var newProject = XCRoot.Create("Library");
//
//             
//             using (var dataContext = new DataContext(config.DatabaseType, config.ConnectionString))
//             {
//                 var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME,
//                     dataContext);
//
//                 newProject.Save(configStorage);
//             }
//             
//             */
//
//             //Сначала проинициализируем основные подсистемы платформы, а уже затем рабочую среду
//
//
//             var savedConfiguration = ConfigurationFactory.Create();
//             IProject currentConfiguration = null;
//
//             using (var dataContext =
//                 new DataContext(config.DatabaseType, config.ConnectionString, IsolationLevel.ReadCommitted))
//             {
//                 var configStorage = new DatabaseFileSystem(DatabaseConstantNames.CONFIG_TABLE_NAME,
//                     dataContext);
//
//
//                 currentConfiguration = null; //Project.Load(configStorage);
//
//                 if (currentConfiguration == null)
//                 {
//                     currentConfiguration = new Project(
//                         new ProjectMD {ProjectName = "Library", ProjectVersion = "1.0.0.0"},
//                         new MDManager(new TypeManager(), new InMemoryUniqueCounter()));
//
//                     savedConfiguration.Save(configStorage);
//                 }
//             }
//
//
//             if (MigrationManager.CheckMigration(currentConfiguration, savedConfiguration))
//             {
//                 DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
//                 MigrationManager.Migrate(currentConfiguration, savedConfiguration);
//
//
//                 var storage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME,
//                     this.DataContextManager.GetContext());
//
//                 savedConfiguration.Save(storage);
//             }
//
//             base.Initialize(config);
//             _logger.Info("TEST Database '{0}' loaded.", Configuration.ProjectName);
//
//             AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
//
//             if (_assemblyManager.CheckConfiguration(Configuration))
//                 _assemblyManager.BuildConfiguration(Configuration, StartupConfig.DatabaseType);
//
//             var asms = _assemblyManager.GetAssemblies(Configuration).First(x => x.Type == AssemblyType.Server);
//
//             var bytes = _assemblyManager.GetAssemblyBytes(asms);
//
//             _logger.Info("Starting load server assembly");
//             var serverAssembly = Assembly.Load(bytes);
//             var serviceType = serverAssembly.GetType("EntryPoint") ??
//                               throw new Exception("Entry point in assembly not defined");
//             var main = serviceType.GetMethod("Main") ?? throw new Exception("Main method not found");
//
//
//             _logger.Info("Run entry point of server assembly");
//             main.Invoke(null, new[] {new object[] {InvokeService, LinkFactory, _startupService}});
//
//             InvokeService.Register(new Route("test"), (c, a) => (int) a[0] + 1);
//             InvokeService.RegisterStream(new Route("stream"), (context, stream, arg) =>
//             {
//                 using (StreamWriter writer = new StreamWriter(stream))
//                 {
//                     writer.WriteLine("dsadsdasdasdasdsadasdsadsd");
//                 }
//             });
//         }
//
//         private ILogger _logger;
//
//         private IServiceProvider _serviceProvider;
//
//         private IUserManager _userManager;
//
//         private IAssemblyManager _assemblyManager;
//         private readonly IStartupService _startupService;
//
//         public override IInvokeService InvokeService { get; }
//
//         public override IAuthenticationManager AuthenticationManager { get; }
//
//
//         /// <summary>
//         /// Сборка конфигурации.
//         /// В сборке хранятся все типы и бизнес логика
//         /// </summary>
//         public Assembly Build { get; set; }
//
//         /// <summary>
//         /// Глобальные объекты
//         /// </summary>
//         public Dictionary<string, object> Globals { get; set; }
//
//
//         /// <summary>
//         /// Менеджеры
//         /// </summary>
//         public IDictionary<Type, IEntityManager> Managers { get; }
//
//         /// <summary>
//         /// Создаёт сессию для пользователя
//         /// </summary>
//         /// <param name="user">Пользователь</param>
//         /// <returns></returns>
//         /// <exception cref="Exception">Если платформа не инициализирована</exception>
//         public override ISession CreateSession(IUser user)
//         {
//             lock (_locking)
//             {
//                 //if (!Sessions.Any()) throw new Exception("The environment not initialized!");
//                 //var id = Sessions.Max(x => x.Id) + 1;
//                 var session = new UserSession(this, user, DataContextManager, CacheService);
//                 Sessions.Add(session);
//
//                 return session;
//             }
//         }
//
//         /// <summary>
//         /// Убить сессию
//         /// </summary>
//         /// <param name="session"></param>
//         public void KillSession(ISession session)
//         {
//             lock (_locking)
//             {
//                 Sessions.Remove(session);
//             }
//         }
//
//         /// <summary>
//         /// Убить сессию
//         /// </summary>
//         /// <param name="id"></param>
//         public void KillSession(Guid id)
//         {
//             lock (_locking)
//             {
//                 var session = Sessions.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Session not found");
//                 Sessions.Remove(session);
//             }
//         }
//
//         /// <summary>
//         /// Зарегистрировать менеджер, который обслуживает определенный тип объекта
//         /// </summary>
//         /// <param name="type"></param>
//         /// <param name="manager"></param>
//         public void RegisterManager(Type type, IEntityManager manager)
//         {
//             Managers.Add(type, manager);
//         }
//
//         /// <summary>
//         /// Отменить регистрацию менеджера, котоырй обслуживает определённый тип объекта
//         /// </summary>
//         /// <param name="type"></param>
//         public void UnregisterManager(Type type)
//         {
//             if (Managers.ContainsKey(type))
//             {
//                 Managers.Remove(type);
//             }
//         }
//
//         /// <summary>
//         /// Получить менеджер по типу сущности
//         /// </summary>
//         /// <param name="type">Тип Entity</param>
//         /// <returns></returns>
//         public IEntityManager GetManager(Type type)
//         {
//             if (Managers.TryGetValue(type, out var manager))
//             {
//                 return manager;
//             }
//
//             throw new Exception($"Manager for type {type.Name} not found");
//         }
//
//         public ILinkFactory LinkFactory { get; }
//     }
}