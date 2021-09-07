﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Aquila.Core.Assemlies;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime;
using Aquila.Runtime.Infrastructure.Helpers;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Instance
{
    /// <summary>
    /// Рабочая среда. Здесь же реализованы все плюшки  манипуляций с данными и так далее
    /// </summary>
    public class DatabaseTestInstance : IPlatformInstance
    {
        private object _locking;

        public DatabaseTestInstance(IInvokeService invokeService, ILinkFactory linkFactory,
            ILogger<DatabaseTestInstance> logger,
            IAuthenticationManager authenticationManager, IServiceProvider serviceProvider,
            DataContextManager contextManager, IUserManager userManager, ICacheService cacheService,
            MigrationManager manager
        )
        {
            _locking = new object();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _userManager = userManager;
            _cacheService = cacheService;

            InvokeService = invokeService;
            LinkFactory = linkFactory;

            MigrationManager = manager;

            Globals = new Dictionary<string, object>();
            AuthenticationManager = authenticationManager;
            DataContextManager = contextManager;

            Sessions = new List<ISession>();
        }

        public DatabaseRuntimeContext DatabaseRuntimeContext { get; private set; }

        public void Initialize(IStartupConfig config)
        {
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
            DatabaseRuntimeContext = DatabaseRuntimeContext.CreateAndLoad(DataContextManager.GetContext());

            _logger.Info("Current configuration was loaded. It contains {0} elements",
                DatabaseRuntimeContext.GetMetadata().Metadata.Count());

            if (MigrationManager.CheckMigration())
            {
                MigrationManager.Migrate();
                DatabaseRuntimeContext = DatabaseRuntimeContext.CreateAndLoad(DataContextManager.GetContext());
            }

            BLAssembly = Assembly.Load(DatabaseRuntimeContext.GetLastAssembly(DataContextManager.GetContext()));
            _logger.Info("Assembly was loaded: {0}", BLAssembly.FullName);

            _logger.Info("Start init assembly");
            var r = BLAssembly.GetRuntimeInit();

            foreach (var item in r)
            {
                switch (item.attr.Kind)
                {
                    case RuntimeInitKind.TypeId:
                        var desc = DatabaseRuntimeContext.GetEntityDescriptor(item.attr.Parameters[0] as string);
                        ((FieldInfo)item.m).SetValue(null, desc.DatabaseId);
                        _logger.Info($"Type id = {desc.DatabaseId}");
                        break;
                    case RuntimeInitKind.SelectQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetLoad(semantic, DatabaseRuntimeContext);
                        _logger.Info($"Gen query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.UpdateQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetSaveUpdate(semantic, DatabaseRuntimeContext);
                        _logger.Info($"Gen query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.InsertQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetSaveInsert(semantic, DatabaseRuntimeContext);
                        _logger.Info($"Gen query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
            _logger.Info("Project '{0}' was loaded.", Name);

            InvokeService.Register(new Route("test"), (c, a) => (int)a[0] + 1);
            InvokeService.RegisterStream(new Route("stream"), (context, stream, arg) =>
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("dsadsdasdasdasdsadasdsadsd");
                }
            });

            Sessions.Add(new SimpleSession(this, new Anonymous()));
        }

        private ILogger _logger;

        private IServiceProvider _serviceProvider;

        private IUserManager _userManager;
        private readonly ICacheService _cacheService;

        public IList<ISession> Sessions { get; }
        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public MigrationManager MigrationManager { get; }

        public string Name => "Library";

        public DataContextManager DataContextManager { get; }
        public Assembly BLAssembly { get; private set; }

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
        public ISession CreateSession(IUser user)
        {
            lock (_locking)
            {
                var session = new UserSession(this, user, DataContextManager, _cacheService);
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