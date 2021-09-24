using System;
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
    /// Platfrom instace
    /// </summary>
    public class PlatformInstance : IPlatformInstance
    {
        private object _locking;

        public PlatformInstance(IInvokeService invokeService, ILinkFactory linkFactory,
            ILogger<PlatformInstance> logger,
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

        private void UpdateDBRContext()
        {
            DatabaseRuntimeContext = DatabaseRuntimeContext.CreateAndLoad(DataContextManager.GetContext());
        }

        private byte[] GetCurrentAssembly()
        {
            return DatabaseRuntimeContext.Files.GetMainAssembly(DataContextManager.GetContext());
        }

        public void Initialize(IStartupConfig config)
        {
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
            UpdateDBRContext();

            _logger.Info("Current configuration was loaded. It contains {0} elements",
                DatabaseRuntimeContext.Metadata.GetMetadata().Metadata.Count());

            if (MigrationManager.CheckMigration())
            {
                MigrationManager.Migrate();
                UpdateDBRContext();
            }

            AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
            _logger.Info("Auth provider was registered");

            LoadAssembly(Assembly.Load(GetCurrentAssembly()));

            _logger.Info("Project '{0}' was loaded.", Name);
        }

        public void UpdateAssembly(Assembly asm)
        {
            LoadAssembly(asm);
        }

        private void LoadAssembly(Assembly asm)
        {
            _logger.Info("[Assembly] Starting init: {0}", asm.FullName);

            BLAssembly = asm;

            _logger.Info("[Assembly] Get tasks for runtime initialization");
            var r = BLAssembly.GetRuntimeInit();

            foreach (var item in r)
            {
                switch (item.attr.Kind)
                {
                    case RuntimeInitKind.TypeId:
                        var desc = DatabaseRuntimeContext.Descriptors.GetEntityDescriptor(item.attr.Parameters[0] as string);
                        ((FieldInfo)item.m).SetValue(null, desc.DatabaseId);
                        _logger.Info($"[Assembly] Set type id for {desc.DatabaseName} = {desc.DatabaseId}");
                        break;
                    case RuntimeInitKind.SelectQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetLoad(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate select query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.UpdateQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetSaveUpdate(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate update query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.InsertQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata().GetSemantic(x => x.FullName == mdFullName);
                        var query = CRUDQueryGenerator.GetSaveInsert(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate insert query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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