using System.IO;
using Aquila.Data;
using Aquila.QueryBuilder;

namespace Aquila.Core.ClientServices
{
    public interface IAdminToolsClientService
    {
        void CreateConfiguration(string name, SqlDatabaseType databaseType, string connectionString);
        void AddConfiguration(SqlDatabaseType databaseType, string connectionString);
    }
}