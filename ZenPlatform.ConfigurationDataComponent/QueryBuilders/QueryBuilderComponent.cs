using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core;
using ZenPlatform.Core.Sessions;
using ZenPlatform.DataComponent.Entity;
using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    /// <summary>
    /// Контракт, который необходимо сделать, чтобы загрузка данных
    /// из\в базу данных была максимально быстрой
    /// </summary>
    public abstract class ObjectToDatabase<T>
        where T : XCObjectTypeBase
    {
        private readonly T _type;

        protected ObjectToDatabase(T type)
        {
            _type = type;
        }

        public T XCType => _type;

        public virtual EntityBase SelectSingleObject(UserSession session, object key)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<EntityBase> SelectMultiplyObjects(UserSession session, object[] keys)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<EntityBase> SelectAllObjects(UserSession session)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateSingleObject(UserSession session, EntityBase entity)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateMultiplyObjects(UserSession session, EntityBase[] entityes)
        {
            throw new NotImplementedException();
        }

        public virtual void InsertSingleObject(UserSession session, EntityBase model)
        {
            throw new NotImplementedException();
        }

        //public virtual DBInsertQuery InsertMultiplyObject(XCObjectTypeBase obj, object[] models)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual DBDeleteQuery DeleteSingleObject(XCObjectTypeBase obj, object key)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual DBDeleteQuery DeleteMultiplyObjects(XCObjectTypeBase obj, object[] keys)
        //{
        //    throw new NotImplementedException();
        //}
    }
}