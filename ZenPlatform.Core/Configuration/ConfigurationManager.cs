using SharpFileSystem.Database;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationManipulator _m;

        public ConfigurationManager(IConfigurationManipulator m)
        {
            _m = m;
        }

        public void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString)
        {
            //Мигрируем...
            MigrationRunner.Migrate(connectionString, databaseType);

            //Создаём пустой проект с именем Project Name

            var newProject = _m.Create(projectName);

            // Необходимо создать контекст данных

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new DatabaseFileSystem(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext);
            var configSaveStorage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext);

            //Сохраняем новоиспечённый проект в сохранённую и конфигураци базы данных
            newProject.Save(configStorage);
            newProject.Save(configSaveStorage);
        }

        public void DeployConfiguration(IProject xcProject, SqlDatabaseType databaseType, string connectionString)
        {
            MigrationRunner.Migrate(connectionString, databaseType);

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new DatabaseFileSystem(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext);
            var configSaveStorage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext);


            xcProject.Save(configStorage);
            xcProject.Save(configSaveStorage);
        }
    }
}