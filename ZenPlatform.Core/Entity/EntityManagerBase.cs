using System.Collections.Generic;

namespace ZenPlatform.Core.Entity
{

    public abstract class EntityManagerBase
    {
        protected EntityManagerBase(Entity2SqlBase sqlProvider, Session session)
        {
            SqlProvider = sqlProvider;
            Session = session;
        }

        protected Entity2SqlBase SqlProvider { get; }
        protected Session Session { get; }
    }


    public abstract class EntityManagerBase<T> : EntityManagerBase
        where T : EntityBase, new()
    {
        protected EntityManagerBase(Entity2SqlBase sqlProvider, Session session) : base(sqlProvider, session)
        {

        }

        //TODO: Сделать async API task 86
   
        public abstract T Create();
        public abstract void Save(T entity);
        public abstract T Load();
        public abstract void Delete(T entity);
        public abstract IEnumerable<T> GetList();



    }
}