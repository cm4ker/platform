using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.Core.Entity
{
    public abstract class SqlBuilder
    {
        public abstract DBSelectQuery GetSelect();
        public abstract DBUpdateQuery GetUpdate();
        public abstract DBDeleteQuery GetDelete();
        public abstract DBInsertQuery GetInsert();
    }
}