using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core.Entity;

namespace ZenPlatform.Core
{
    public class PlatformEnvironment
    {
        private object _locking;

        public PlatformEnvironment()
        {
            _locking = new object();

            Sessions = new List<Session>();
            Globals = new Dictionary<string, object>();
            Managers = new Dictionary<Type, EntityManagerBase>();
            Entityes = new Dictionary<Guid, EntityDefinition>();
        }



        public Dictionary<string, object> Globals { get; set; }

        public IList<Session> Sessions { get; }

        public IDictionary<Guid, EntityDefinition> Entityes { get; }

        public IDictionary<Type, EntityManagerBase> Managers { get; }



        public Session CreateSession()
        {
            lock (_locking)
            {
                var id = Sessions.Max(x => x.Id) + 1;
                var session = new Session(this, id);

                Sessions.Add(session);

                return session;
            }
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



        public void RegisterManager(Type type, EntityManagerBase manager)
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

        public EntityManagerBase GetManager(Type type)
        {
            if (Managers.TryGetValue(type, out var manager))
            {
                return manager;
            }

            throw new Exception($"Manager for type {type.Name} not found");
        }

        public void RegisterEntity(EntityDefinition definition)
        {
            Entityes.Add(definition.Key, definition);
        }

        public EntityDefinition GetDefinition(Guid key)
        {
            if (Entityes.TryGetValue(key, out var entityDefinition))
            {
                return entityDefinition;
            }

            throw new Exception($"Entity definition {key} not found");
        }

        public EntityDefinition GetDefinition(Type entityType)
        {
            var entityDefinition = Entityes.First(x => x.Value.EntityType == entityType).Value;
            return entityDefinition;
        }
    }

    public class EntityDefinition
    {
        public EntityDefinition(PObjectType entityConfig, Type entityType, Type dtoType)
        {
            EntityConfig = entityConfig;
            EntityType = entityType;
            DtoType = dtoType;

        }

        public Guid Key => EntityConfig.Id;
        public PObjectType EntityConfig { get; }
        public Type EntityType { get; }
        public Type DtoType { get; }
    }
}
