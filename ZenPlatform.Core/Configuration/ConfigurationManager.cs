﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;

namespace ZenPlatform.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        public ConfigurationManager()
        {

        }

        public void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString)
        {

            //Мигрируем...
            MigrationRunner.Migrate(connectionString, databaseType);

            //Создаём пустой проект с именем Project Name

            var newProject = XCRoot.Create(projectName);

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

        public void DeployConfiguration(XCRoot xcRoot, SqlDatabaseType databaseType, string connectionString)
        {


            MigrationRunner.Migrate(connectionString, databaseType);

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext,
               SqlCompillerBase.FormEnum(databaseType));

            var configSaveStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));


            xcRoot.Save(configStorage);
            xcRoot.Save(configSaveStorage);
        }




    }
}
