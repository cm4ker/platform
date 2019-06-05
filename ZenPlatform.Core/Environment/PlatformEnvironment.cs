using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.ServerClientShared.Tools;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    ///  Базовый класс среды, служит для того, чтобы описать две производные среды <see cref="WorkEnvironment"/> и <see cref="SystemEnvironment"/>
    /// </summary>
    public abstract class PlatformEnvironment : IEnvironment
    {
        

        protected PlatformEnvironment(IDataContextManager dataContextManager)
        {
            Sessions = new RemovingList<ISession>();
            
            DataContextManager = dataContextManager;

        }

        protected SystemSession SystemSession { get; private set; }

        public virtual void Initialize(StartupConfig config)
        {

            StartupConfig = config;

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);

            SystemSession = new SystemSession(this, DataContextManager);
            Sessions.Add(SystemSession);

            //TODO: Дать возможность выбрать, какую конфигурацию загружать, с базы данных или из файловой системы

            var storage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, DataContextManager.GetContext(),
                DataContextManager.SqlCompiler);

            Configuration = XCRoot.Load(storage);
        }

        public abstract EntityMetadata GetMetadata(Guid key);
        public abstract EntityMetadata GetMetadata(Type type);
        public abstract IEntityManager GetManager(Type type);
        public abstract ISession CreateSession(IUser user);

        /// <summary>
        /// Конфигурация базы данных
        /// </summary>
        public XCRoot Configuration { get; private set; }

        /// <summary>
        /// Сессии
        /// </summary>
        public IList<ISession> Sessions { get; }

        /// <summary>
        /// Стартовая конфигурация
        /// </summary>
        public StartupConfig StartupConfig { get; private set; }

        public virtual IInvokeService InvokeService => throw new System.NotImplementedException();

        public virtual IAuthenticationManager AuthenticationManager => throw new System.NotImplementedException();

        public IDataContextManager DataContextManager { get; private set; }
    }
}