using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Logging;

namespace Aquila.Core.Instance
{
    /// <summary>
    ///  Базовый класс среды, служит для того, чтобы описать две производные среды <see cref="WorkInstance"/> и <see cref="SystemInstance"/>
    /// </summary>
    public sealed class PlatformInstance : IPlatformInstance
    {
        private ILogger _logger;

        private IServiceProvider _serviceProvider;

        private IUserManager _userManager;

        private object _locking;

        protected ICacheService CacheService;
        protected IServiceProvider ServiceProvider;


        protected PlatformInstance(DataContextManager dataContextManager, ICacheService cacheService,
            IServiceProvider serviceProvider, ILogger<PlatformInstance> logger, IUserManager userManager)
        {
            Sessions = new List<ISession>();
            DataContextManager = dataContextManager;
            CacheService = cacheService;

            _locking = new object();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _userManager = userManager;
        }

        protected SystemSession SystemSession { get; private set; }

        public void Initialize(IStartupConfig config)
        {
            StartupConfig = config;

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);

            SystemSession = new SystemSession(this, DataContextManager, CacheService);
            Sessions.Add(SystemSession);
        }

        /// <summary>
        /// Сессии
        /// </summary>
        public IList<ISession> Sessions { get; }

        /// <summary>
        /// Стартовая конфигурация
        /// </summary>
        public IStartupConfig StartupConfig { get; private set; }

        public DataContextManager DataContextManager { get; private set; }

        public Assembly BLAssembly { get; protected set; }

        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public string Name => throw new NotImplementedException();


        /// <summary>
        /// Глобальные объекты
        /// </summary>
        public Dictionary<string, object> Globals { get; set; }

        // /// <summary>
        // /// Менеджеры
        // /// </summary>
        // public IDictionary<Type, IEntityManager> Managers { get; }

        /// <summary>
        /// Создаёт сессию для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если платформа не инициализирована</exception>
        public ISession CreateSession(IUser user)
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

        // /// <summary>
        // /// Зарегистрировать менеджер, который обслуживает определенный тип объекта
        // /// </summary>
        // /// <param name="type"></param>
        // /// <param name="manager"></param>
        // public void RegisterManager(Type type, IEntityManager manager)
        // {
        //     Managers.Add(type, manager);
        // }
        //
        // /// <summary>
        // /// Отменить регистрацию менеджера, котоырй обслуживает определённый тип объекта
        // /// </summary>
        // /// <param name="type"></param>
        // public void UnregisterManager(Type type)
        // {
        //     if (Managers.ContainsKey(type))
        //     {
        //         Managers.Remove(type);
        //     }
        // }

        // /// <summary>
        // /// Получить менеджер по типу сущности
        // /// </summary>
        // /// <param name="type">Тип Entity</param>
        // /// <returns></returns>
        // public IEntityManager GetManager(Type type)
        // {
        //     if (Managers.TryGetValue(type, out var manager))
        //     {
        //         return manager;
        //     }
        //
        //     throw new Exception($"Manager for type {type.Name} not found");
        // }

        public ILinkFactory LinkFactory { get; }
    }
}