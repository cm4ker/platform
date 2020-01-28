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

            var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            var configSaveStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            //Сохраняем новоиспечённый проект в сохранённую и конфигураци базы данных
            newProject.Save(configStorage);
            newProject.Save(configSaveStorage);
        }

        public void DeployConfiguration(IProject xcProject, SqlDatabaseType databaseType, string connectionString)
        {
            MigrationRunner.Migrate(connectionString, databaseType);

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            var configSaveStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));


            xcProject.Save(configStorage);
            xcProject.Save(configSaveStorage);
        }
    }
}