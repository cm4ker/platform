using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Configuration
{
    public interface IConfigurationManager
    {
        void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString);
        void DeployConfiguration(IXCRoot xcRoot, SqlDatabaseType databaseType, string connectionString);
    }
}