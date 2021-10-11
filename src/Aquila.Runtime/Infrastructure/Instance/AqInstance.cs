using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    public class AqInstance
    {
        private object _locking;

        public AqInstance(IInvokeService invokeService, ILogger<AqInstance> logger,
            IAuthenticationManager authenticationManager, IServiceProvider serviceProvider,
            DataContextManager contextManager, UserManager userManager, ICacheService cacheService,
            MigrationManager manager
        )
        {
            _locking = new object();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _userManager = userManager;
            _cacheService = cacheService;

            InvokeService = invokeService;

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

        public void Initialize(StartupConfig config)
        {
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
            UpdateDBRContext();

            _logger.Info("Current configuration was loaded. It contains {0} elements",
                DatabaseRuntimeContext.Metadata.GetMetadata().Metadata.Count());

            // AuthenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
            // _logger.Info("Auth provider was registered");

            var currentAssembly = GetCurrentAssembly();

            if (currentAssembly != null)
            {
                LoadAssembly(Assembly.Load(currentAssembly));
                _logger.Info("Project '{0}' was loaded.", Name);
            }
            else
                _logger.Info("Assembly is empty");
        }


        public bool PendingChanges => MigrationManager.CheckMigration();

        public void Migrate()
        {
            if (PendingChanges)
            {
                MigrationManager.Migrate();
                UpdateDBRContext();
            }
        }

        public void Deploy(Stream packageStream)
        {
            byte[] ReadFully(Stream input)
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    return ms.ToArray();
                }
            }

            ZipArchive arch = new ZipArchive(packageStream, ZipArchiveMode.Read);

            var dc = DataContextManager.GetContext();

            DatabaseRuntimeContext.PendingMetadata.Clear(dc);
            DatabaseRuntimeContext.PendingFiles.Clear(dc);

            foreach (var item in arch.Entries)
            {
                if (item.FullName.StartsWith("Metadata\\"))
                {
                    using var stream = item.Open();
                    using StreamReader sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    DatabaseRuntimeContext.PendingMetadata.GetMetadata().AddMetadata(EntityMetadata.FromYaml(content));
                }
                else
                {
                    var descr = new FileDescriptor
                    {
                        Name = item.FullName,
                        Type = FileType.Unknown,
                        CreateDateTime = DateTime.Now
                    };

                    using var stream = item.Open();
                    DatabaseRuntimeContext.PendingFiles.SaveFile(dc, descr,
                        ReadFully(stream));
                }
            }

            DatabaseRuntimeContext.PendingMetadata.SaveMetadata(dc);
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
                        var desc = DatabaseRuntimeContext.Descriptors.GetEntityDescriptor(
                            item.attr.Parameters[0] as string);

                        if (desc is null)
                            break;

                        ((FieldInfo)item.m).SetValue(null, desc.DatabaseId);
                        _logger.Info($"[Assembly] Set type id for {desc.DatabaseName} = {desc.DatabaseId}");
                        break;
                    case RuntimeInitKind.SelectQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);

                        if (semantic is null)
                            break;

                        var query = CRUDQueryGenerator.GetLoad(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate select query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.UpdateQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);

                        if (semantic is null)
                            break;

                        var query = CRUDQueryGenerator.GetSaveUpdate(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate update query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.InsertQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);
                        if (semantic is null)
                            break;

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

        private UserManager _userManager;
        private readonly ICacheService _cacheService;

        public IList<ISession> Sessions { get; }
        public IInvokeService InvokeService { get; }

        public IAuthenticationManager AuthenticationManager { get; }

        public MigrationManager MigrationManager { get; }

        public string Name => "Library";

        public DataContextManager DataContextManager { get; }

        public Assembly BLAssembly { get; private set; }


        /// <summary>
        /// Global objects
        /// </summary>
        public Dictionary<string, object> Globals { get; set; }

        /// <summary>
        /// Создаёт сессию для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если платформа не инициализирована</exception>
        public ISession CreateSession(AqUser user)
        {
            lock (_locking)
            {
                var session = new UserSession(this, user, DataContextManager, _cacheService);
                Sessions.Add(session);

                return session;
            }
        }

        /// <summary>
        /// Force end session
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