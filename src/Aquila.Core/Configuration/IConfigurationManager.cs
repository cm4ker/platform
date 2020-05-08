using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;
using Aquila.QueryBuilder;

namespace Aquila.Core.Configuration
{
    public interface IConfigurationManager
    {
        void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString);
        void DeployConfiguration(IProject xcProject, SqlDatabaseType databaseType, string connectionString);
    }
}