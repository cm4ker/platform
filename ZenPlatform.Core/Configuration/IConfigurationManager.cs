using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Configuration
{
    public interface IConfigurationManager
    {
        void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString);
        void DeployConfiguration(XCRoot xcRoot, SqlDatabaseType databaseType, string connectionString);
    }
}