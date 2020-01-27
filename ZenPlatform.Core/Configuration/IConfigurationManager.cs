using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Configuration
{
    public interface IConfigurationManager
    {
        void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString);
        void DeployConfiguration(IRoot xcRoot, SqlDatabaseType databaseType, string connectionString);
    }
}