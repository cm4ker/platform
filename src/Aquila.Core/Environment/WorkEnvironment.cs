using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Logging;
using Aquila.Core.Network;
using Aquila.Core.Sessions;
using Aquila.Core.Tools;
using Aquila.Data;
using Aquila.Core.Assemlies;
using Aquila.Core.Crypto;
using Aquila.Configuration.Structure;
using Aquila.Initializer;
using Aquila.Core.Assemblies;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Data.Entity;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.Network;

namespace Aquila.Core.Environment
{
    /// <summary>
    /// Рабочая среда. Здесь же реализованы все плюшки  манипуляций с данными и так далее
    /// </summary>
    public class WorkEnvironment : PlatformEnvironment, IWorkEnvironment
    {
        private object _locking;

        public WorkEnvironment(IInvokeService invokeService, ILinkFactory linkFactory, ILogger<WorkEnvironment> logger,
            IAuthenticationManager authenticationManager, IServiceProvider serviceProvider,
            IDataContextManager contextManager, IUserManager userManager, ICacheService cacheService,
            IAssemblyManager assemblyManager, IConfigurationManipulator manipulator) :
            base(contextManager, cacheService, manipulator)
        {
            _locking = new object();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _userManager = userManager;
            InvokeService = invokeService;
            LinkFactory = linkFactory;
            _assemblyManager = assemblyManager;

            Globals = new Dictionary<string, object>();

            Managers = new Dictionary<Type, IEntityManager>();

            AuthenticationManager = authenticationManager;
        }

        /// <summary>
        /// Инициализация среды.
        /// На этом этапе происходит создание подключения к базе
        /// Загрузка конфигурации и так далее
        /// </summary>
        public override void Initialize(IStartupConfig config)
        {
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);
            //Сначала проинициализируем основные подсистемы платформы, а уже затем рабочую среду
            base.Initialize(config);
            _logger.Info("Database '{0}' loaded.", Configuration.ProjectName);

            AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));

            if (_assemblyManager.CheckConfiguration(Configuration))
                _assemblyManager.BuildConfiguration(Configuration, StartupConfig.DatabaseType);


            /*
            //TODO: получить библиотеку с сгенерированными сущностями dto и так далее
            Build = Assembly.LoadFile("");

           

            //Зарегистрируем все даные
            foreach (var type in Configuration.Data.ComponentTypes)
            {
                var componentImpl = type.Parent.ComponentImpl;

                var manager = componentImpl.Manager;

                var className = componentImpl.Generator.GetEntityClassName(type);
                var dtoClassName = componentImpl.Generator.GetDtoClassName(type);
                var csEntityType = Build.GetType(className);
                var csDtoType = Build.GetType(dtoClassName);

                RegisterManager(csEntityType, manager);
                RegisterEntity(new EntityMetadata(type, csEntityType, csDtoType));
            }
            */
        }

        private ILogger _logger;

        private IServiceProvider _serviceProvider;

        private IUserManager _userManager;

        private IAssemblyManager _assemblyManager;

        public override IInvokeService InvokeService { get; }

        public override IAuthenticationManager AuthenticationManager { get; }


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
        /// Менеджеры
        /// </summary>
        public IDictionary<Type, IEntityManager> Managers { get; }

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

        /// <summary>
        /// Зарегистрировать менеджер, который обслуживает определенный тип объекта
        /// </summary>
        /// <param name="type"></param>
        /// <param name="manager"></param>
        public void RegisterManager(Type type, IEntityManager manager)
        {
            Managers.Add(type, manager);
        }

        /// <summary>
        /// Отменить регистрацию менеджера, котоырй обслуживает определённый тип объекта
        /// </summary>
        /// <param name="type"></param>
        public void UnregisterManager(Type type)
        {
            if (Managers.ContainsKey(type))
            {
                Managers.Remove(type);
            }
        }

        /// <summary>
        /// Получить менеджер по типу сущности
        /// </summary>
        /// <param name="type">Тип Entity</param>
        /// <returns></returns>
        public IEntityManager GetManager(Type type)
        {
            if (Managers.TryGetValue(type, out var manager))
            {
                return manager;
            }

            throw new Exception($"Manager for type {type.Name} not found");
        }

        public ILinkFactory LinkFactory { get; }
    }
}