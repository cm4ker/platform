using Aquila.Configuration.Structure;
using Aquila.Core.Contracts;
using Aquila.QueryBuilder;

namespace Aquila.Core.Configuration
{
    public interface IConfigurationManager
    {
        void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString);
        void DeployConfiguration(IProject xcProject, SqlDatabaseType databaseType, string connectionString);
    }
}