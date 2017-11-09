using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.Core.Entity
{
    public abstract class Entity2SqlBase
    {
        public abstract DBSelectQuery GetSelect();
        public abstract DBUpdateQuery GetUpdate();
        public abstract DBDeleteQuery GetDelete();
        public abstract DBInsertQuery GetInsert();
    }
}