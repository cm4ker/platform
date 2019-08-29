using System.IO;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.ClientServices
{
    public interface IAdminToolsClientService
    {
        void BuildConfiguration(string name);
        void CreateConfiguration(string name, SqlDatabaseType databaseType, string connectionString);
        void AddConfiguration(SqlDatabaseType databaseType, string connectionString);

    }
}