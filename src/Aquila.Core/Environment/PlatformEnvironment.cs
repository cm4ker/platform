using System;
using System.Collections.Generic;
using SharpFileSystem.Database;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Configuration;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Network;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.QueryBuilder;
using Aquila.Core.Tools;

namespace Aquila.Core.Environment
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