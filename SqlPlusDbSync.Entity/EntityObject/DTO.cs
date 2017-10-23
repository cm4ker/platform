using System;
using System.Dynamic;
using SqlPlusDbSync.Platform.EntityObject;

namespace SqlPlusDbSync.Entity.EntityObject
{
    public class DTO
    {
        public DTO()
        {

        }
    }

    //public class Entity
    //{
    //    private DTO _dto;
    //    private readonly SObjectEntityManager _manager;

    //    public Entity(DTO dto, SObjectEntityManager manager)
    //    {
    //        _dto = dto;
    //        _manager = manager;

    //        DynamicProperties = new ExpandoObject();
    //    }

    //    public Entity Link => this;

    //    public dynamic DynamicProperties;

    //    public object GetKey => _dto.Key;

    //    public void Save()
    //    {
    //        _manager.Save(_dto);
    //    }

    //    public void Reload()
    //    {
    //        _dto = _manager.Load(_dto.Key);
    //    }

    //    public void Delete()
    //    {
    //        _manager.Delete(_dto.Key);
    //    }
    //}

    //public static class Session
    //{
    //    public static AsnaDatabaseContext Context;
    //}

    //public class EntityLinkFactory
    //{
    //    public static Entity Create(SType sobject)
    //    {
    //        SObjectEntityManager entityManager = new SObjectEntityManager(Session.Context, sobject);

    //        return new Entity(entityManager.Create(), entityManager);
    //    }

    //    public static Entity Create(DTO dto, SType sobject)
    //    {
    //        SObjectEntityManager entityManager = new SObjectEntityManager(Session.Context, sobject);

    //        return new Entity(dto, entityManager);
    //    }
    //}
}
