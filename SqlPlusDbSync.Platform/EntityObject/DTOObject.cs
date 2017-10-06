using System.Collections.Generic;
using System.Dynamic;
using QueryCompiler.Schema;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Platform.EntityObject
{
    public class DTOObject
    {
        public DTOObject()
        {
            DynamicProperties = new ExpandoObject();
            Register = true;
        }

        public object Key { get; set; }
        public object Version { get; set; }

        public bool IsDeleted { get; set; }
        public bool Register { get; set; }

        public dynamic DynamicProperties;


        public override bool Equals(object obj)
        {
            var typeequal = this.GetType() == obj.GetType();
            if (!typeequal) return false;

            if (obj is DTOObject)
                return this.Key.Equals(((DTOObject)obj).Key);
            return false;
        }

        public override int GetHashCode()
        {
            return 990326508 + EqualityComparer<object>.Default.GetHashCode(Key);
        }
    }

    public class Entity
    {
        private DTOObject _dtoObject;
        private readonly SObjectEntityManager _manager;

        public Entity(DTOObject dtoObject, SObjectEntityManager manager)
        {
            _dtoObject = dtoObject;
            _manager = manager;
        }

        public Entity Link => this;

        public object GetKey => _dtoObject.Key;

        public void Save()
        {
            _manager.Save(_dtoObject);
        }

        public void Reload()
        {
            _dtoObject = _manager.Load(_dtoObject.Key);
        }

        public void Delete()
        {
            _manager.Delete(_dtoObject.Key);
        }
    }

    public static class Session
    {
        public static AsnaDatabaseContext Context;
    }

    public class EntityLinkFactory
    {
        public static Entity Create(SType sobject)
        {
            SObjectEntityManager entityManager = new SObjectEntityManager(Session.Context, sobject);

            return new Entity(entityManager.Create(), entityManager);
        }

        public static Entity Create(DTOObject dtoObject, SType sobject)
        {
            SObjectEntityManager entityManager = new SObjectEntityManager(Session.Context, sobject);

            return new Entity(dtoObject, entityManager);
        }
    }
}
