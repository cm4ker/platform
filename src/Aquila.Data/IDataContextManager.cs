using Aquila.QueryBuilder;

namespace Aquila.Data
{
    public interface IDataContextManager
    {
        DataContext GetContext();
        
        void Initialize(SqlDatabaseType dbType, string connectionString);

        ISqlCompiler SqlCompiler { get; }

        SqlDatabaseType DatabaseType { get; }
    }
}