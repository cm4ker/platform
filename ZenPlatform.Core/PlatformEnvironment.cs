using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Contracts.Entity;


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
         */

        public PlatformEnvironment()
        {
            _locking = new object();

            Sessions = new List<Session>();
            Globals = new Dictionary<string, object>();

            Managers = new Dictionary<Type, IEntityManager>();
            Entityes = new Dictionary<Guid, EntityMetadata>();
        }

        /// <summary>
        /// Инициализация среды.
        /// На этом этапе происходит создание подключения к базе
        /// Загрузка конфигурации и так далее
        /// </summary>
        public void Initialize()
        {
            _systemSession = new SystemSession(this, 1);

            Sessions.Add(_systemSession);

            foreach (var type in Root.Data.ComponentTypes)
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

            //TODO: добавить проверку миграций
        }

        public XCRoot Root { get; set; }


        /// <summary>
        /// Сборка конфигурации.
        /// В сборке хранятся все типы и бизнес логика
        /// </summary>
        public Assembly Build { get; set; }

        public Dictionary<string, object> Globals { get; set; }

        public IList<Session> Sessions { get; }

        public IDictionary<Guid, EntityMetadata> Entityes { get; }

        public IDictionary<Type, IEntityManager> Managers { get; }

        public Session CreateSession()
        {
            lock (_locking)
            {
                var id = (Sessions.Count == 0) ? 1 : Sessions.Max(x => x.Id) + 1;
                var session = new Session(this, id);

                Sessions.Add(session);

                return session;
            }
        }

        public SystemSession GetSystemSession()
        {
            if (_systemSession is null)
                throw new InvalidOperationException("System session is not created. The system go down.");

            return _systemSession;
        }

        public void RemoveSession(Session session)
        {
            lock (_locking)
            {
                Sessions.Remove(session);
            }
        }

        public void RemoveSession(int id)
        {
            lock (_locking)
            {
                var session = Sessions.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Session not found");
                Sessions.Remove(session);
            }
        }

        public void RegisterManager(Type type, IEntityManager manager)
        {
            Managers.Add(type, manager);
        }

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

        public void RegisterEntity(EntityMetadata metadata)
        {
            Entityes.Add(metadata.Key, metadata);
        }

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