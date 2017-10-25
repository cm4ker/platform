using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.ConfigurationDataComponent
{
    public abstract class Entity2SqlBase
    {
        public abstract DBSelectQuery GetSelect();
        public abstract DBUpdateQuery GetUpdate();
        public abstract DBDeleteQuery GetDelete();
        public abstract DBInsertQuery GetInsert();
    }
}