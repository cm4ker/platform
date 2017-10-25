using System.Collections.Generic;
using ZenPlatform.ConfigurationDataComponent;

namespace ZenPlatform.DataComponent
{

    public abstract class EntityManagerBase
    {
        protected EntityManagerBase(Entity2SqlBase sqlProvider)
        {
            SqlProvider = sqlProvider;
        }

        protected Entity2SqlBase SqlProvider { get; }
    }


    public abstract class EntityManagerBase<T> : EntityManagerBase
        where T : EntityBase, new()
    {
        protected EntityManagerBase(Entity2SqlBase sqlProvider) : base(sqlProvider)
        {

        }

        public abstract T Create();
        public abstract void Save(T entity);
        public abstract T Load();
        public abstract void Delete(T entity);
        public abstract IEnumerable<T> GetList();
    }
}