using System;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public abstract class QueryBuilderComponent<T>
        where T : XCObjectTypeBase
    {
        public virtual DBSelectQuery SelectSingleObject(T obj)
        {
            throw new NotImplementedException();
        }

        public virtual DBSelectQuery SelectMultiplyObjects()
        {
            throw new NotImplementedException();
        }

        public virtual DBUpdateQuery UpdateSingleObject()
        {
            throw new NotImplementedException();
        }

        public virtual DBUpdateQuery UpdateMultiplyObjects()
        {
            throw new NotImplementedException();
        }

        public virtual DBInsertQuery InsertObject()
        {
            throw new NotImplementedException();
        }

        public virtual DBDeleteQuery DeleteSingleObject()
        {
            throw new NotImplementedException();
        }

        public virtual DBDeleteQuery DeleteMultiplyObjects()
        {
            throw new NotImplementedException();
        }
    }
}