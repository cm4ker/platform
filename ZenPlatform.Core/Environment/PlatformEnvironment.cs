using System;
using System.Collections.Generic;
using SharpFileSystem.Database;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    ///  Базовый класс среды, служит для того, чтобы описать две производные среды <see cref="WorkEnvironment"/> и <see cref="SystemEnvironment"/>
    /// </summary>
    public abstract class PlatformEnvironment : IPlatformEnvironment
    {
        protected ICacheService CacheService;
        private readonly IConfigurationManipulator _m;
        protected IServiceProvider ServiceProvider;

        protected PlatformEnvironment(IDataContextManager dataContextManager, ICacheService cacheService,
            IConfigurationManipulator m)
        {
            Sessions = new List<ISession>();
            DataContextManager = dataContextManager;
            CacheService = cacheService;
            _m = m;
        }

        protected SystemSession SystemSession { get; private set; }

        public virtual void Initialize(IStartupConfig config)
        {
            StartupConfig = config;

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);

            SystemSession = new SystemSession(this, DataContextManager, CacheService);
            Sessions.Add(SystemSession);

            //TODO: Дать возможность выбрать, какую конфигурацию загружать, с базы данных или из файловой системы

            var storage = new DatabaseFileSystem(DatabaseConstantNames.CONFIG_TABLE_NAME,
                DataContextManager.GetContext());

            Configuration = _m.Load(storage);
        }

        public abstract ISession CreateSession(IUser user);

        /// <summary>
        /// Конфигурация базы данных
        /// </summary>
        public IProject Configuration { get; private set; }

        /// <summary>
        /// Сессии
        /// </summary>
        public IList<ISession> Sessions { get; }

        /// <summary>
        /// Стартовая конфигурация
        /// </summary>
        public IStartupConfig StartupConfig { get; private set; }


        public IDataContextManager DataContextManager { get; private set; }

        public abstract IInvokeService InvokeService { get; }

        public abstract IAuthenticationManager AuthenticationManager { get; }

        public string Name => Configuration.ProjectName;
    }
}