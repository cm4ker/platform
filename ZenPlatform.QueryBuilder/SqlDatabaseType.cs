namespace ZenPlatform.QueryBuilder
{
    public enum SqlDatabaseType
    {
        SqlServer,
        Postgres,
        MySql,
        Oracle
    }

    public class SqlCompillerBase
    {
        public static ISqlCompiler FormEnum(SqlDatabaseType dbtype)
        {
            return null;
        }
    }
}