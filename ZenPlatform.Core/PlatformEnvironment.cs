using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Contracts.Entity;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Configuration;
using ZenPlatform.QueryBuilder;


namespace ZenPlatform.Core
{
    public class PlatformEnvironment
    {
        private object _locking;

        private SystemSession _systemSession;

        /*
         *  Среда должна обеспечиватьдоступ к конфигурации. Так как именно в среду будет загружаться конфигурация 
         *  
         *  
         *  Необходимо реализовать следующий интерфейс:
         *      
         *      Env.ConfigurationManager.Load(string path)      -- Загружает конфигурацию из каталога
         *      Env.ConfigurationManager.LoadDb()               -- Загружает конфигурацию базы данных
         *      Env.ConfigurationManager.UnLoad(string path)    -- Выгружает конфигурацию конфигурацию
         *      Env.ConfigurationManager.Apply()                -- Применяет текущую загруженную конфигурацию, в этот момент применяются все изменения
         *
         *
         *
         * System session. Не должна быть инкапсулирована в PlatformEnvironment
         * по той причине, что у нас будет несколько ProcessWorker'ов (PS)
         * и их нужно всех синхронизировать между собой, чтобы изменять конфигурацию
         *
         * Простое решение - инкапсулировать SystemSession внутри SystemProcessWorker это позволит
         * запускать новые PS после изменения базы данных 
         * 
         */

        public PlatformEnvironment(StartupConfig config)
        {
            _locking = new object();

            Sessions = new List<Session>();
            Globals = new Dictionary<string, object>();

            Managers = new Dictionary<Type, IEntityManager>();
            Entityes = new Dictionary<Guid, EntityMetadata>();

            SqlCompiler = SqlCompillerBase.FormEnum(config.DatabaseType);

            StartupConfig = config;
        }

        /// <summary>
        /// Инициализация среды.
        /// На этом этапе происходит создание подключения к базе
        /// Загрузка конфигурации и так далее
        /// </summary>
        public void Initialize()
        {
            //_systemSession = new SystemSession(this, 1);

            //TODO: Дать возможность выбрать, какую конфигурацию загружать, с базы данных или из файловой системы
            //заглушка
            var storage = new XCDatabaseStorage("conf", _systemSession.GetDataContext(), SqlCompiler);

            Configuration = XCRoot.Load(storage);

            Sessions.Add(_systemSession);

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
        }

        /// <summary>
        /// Конфигурация
        /// </summary>
        public XCRoot Configuration { get; private set; }

        public StartupConfig StartupConfig { get; }

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
        /// Сессии
        /// </summary>
        public IList<Session> Sessions { get; }


        /// <summary>
        /// Сущности
        /// </summary>
        public IDictionary<Guid, EntityMetadata> Entityes { get; }

        /// <summary>
        /// Менеджеры
        /// </summary>
        public IDictionary<Type, IEntityManager> Managers { get; }


        /// <summary>
        /// Компилятор запросов, определяется на этапе инициализации приложения
        /// </summary>
        public SqlCompillerBase SqlCompiler { get; }

        /// <summary>
        /// Создаёт сессию для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если платформа не инициализирована</exception>
        public Session CreateSession(User user)
        {
            lock (_locking)
            {
                if (!Sessions.Any()) throw new Exception("The environment not initialized!");

                var id = Sessions.Max(x => x.Id) + 1;

                var session = new UserSession(this, user, id);

                Sessions.Add(session);

                return session;
            }
        }

        /// <summary>
        /// Получить системную сессию
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public SystemSession GetSystemSession()
        {
            if (_systemSession is null)
                throw new InvalidOperationException("System session is not created. The system go down.");

            return _systemSession;
        }

        /// <summary>
        /// Убить сессию
        /// </summary>
        /// <param name="session"></param>
        public void KillSession(Session session)
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
        public void KillSession(int id)
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

        /// <summary>
        /// Зарегистрировать метаданные сущности
        /// </summary>
        /// <param name="metadata"></param>
        public void RegisterEntity(EntityMetadata metadata)
        {
            Entityes.Add(metadata.Key, metadata);
        }


        /// <summary>
        /// Получить метаданные сущности по её ключу
        /// </summary>
        /// <param name="key">Ключ типа сущности</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public EntityMetadata GetMetadata(Guid key)
        {
            if (Entityes.TryGetValue(key, out var entityDefinition))
            {
                return entityDefinition;
            }

            throw new Exception($"Entity definition {key} not found");
        }

        /// <summary>
        /// Получить описание по типу
        /// </summary>
        /// <param name="type">Типом может быть объект DTO или объект Entity</param>
        /// <returns></returns>
        public EntityMetadata GetMetadata(Type type)
        {
            var entityDefinition = Entityes.First(x => x.Value.EntityType == type || x.Value.DtoType == type).Value;
            return entityDefinition;
        }
    }
}