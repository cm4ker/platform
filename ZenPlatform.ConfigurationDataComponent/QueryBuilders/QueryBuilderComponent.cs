using System;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    /// <summary>
    /// Контракт, который необходимо сделать, чтобы обеспечить все доступные операции
    /// </summary>
    public abstract class QueryBuilderComponentBase
    {
        public virtual DBSelectQuery SelectSingleObject(XCObjectTypeBase obj, object key)
        {
            throw new NotImplementedException();
        }

        public virtual DBSelectQuery SelectMultiplyObjects(XCObjectTypeBase obj, object[] keys)
        {
            throw new NotImplementedException();
        }

        public virtual DBUpdateQuery UpdateSingleObject(XCObjectTypeBase obj, object model)
        {
            throw new NotImplementedException();
        }

        public virtual DBUpdateQuery UpdateMultiplyObjects(XCObjectTypeBase obj, object[] models)
        {
            throw new NotImplementedException();
        }

        public virtual DBInsertQuery InsertSingleObject(XCObjectTypeBase obj, object model)
        {
            throw new NotImplementedException();
        }

        public virtual DBInsertQuery InsertMultiplyObject(XCObjectTypeBase obj, object[] models)
        {
            throw new NotImplementedException();
        }

        public virtual DBDeleteQuery DeleteSingleObject(XCObjectTypeBase obj, object key)
        {
            throw new NotImplementedException();
        }

        public virtual DBDeleteQuery DeleteMultiplyObjects(XCObjectTypeBase obj, object[] keys)
        {
            throw new NotImplementedException();
        }
    }
}