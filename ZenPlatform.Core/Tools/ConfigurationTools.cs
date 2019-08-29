using System;
using System.Collections.Generic;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Data;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;
using ZenPlatform.Compiler.Platform;
using System.IO;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Core.Helpers;
using ZenPlatform.QueryBuilder.DML.Insert;
using System.Data.Common;
using System.Linq;

namespace ZenPlatform.Core.Tools
{

    public static class ConfigurationTools
    {

        public static void CreateConfiguration(string projectName, SqlDatabaseType databaseType, string connectionString, bool createIfNotExists)
        {


            if (createIfNotExists)
            {
                var sqlCompiller = SqlCompillerBase.FormEnum(databaseType);

                DataContext dc = new DataContext(databaseType, connectionString);

                CreateDatabaseQueryNode cDatabase = new CreateDatabaseQueryNode();
            }
            

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

        public static void DeployConfiguration(XCRoot xcRoot, SqlDatabaseType databaseType, string connectionString)
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


        public static void SaveAssembly(string name, string configurationHash, Stream stream,
            SqlCompillerBase compiler, DataContext context)
        {


            var query = new InsertQueryNode()
               .InsertInto("assemblies")
               .WithFieldAndValue(x => x.Field("assembly_hash"),
                   x => x.Parameter("assembly_hash"))
               .WithFieldAndValue(x => x.Field("configuration_hash"),
                   x => x.Parameter("configuration_hash"))
               .WithFieldAndValue(x => x.Field("name"),
                   x => x.Parameter("name"))
               .WithFieldAndValue(x => x.Field("data"),
                   x => x.Parameter("data"));

            using (var cmd = context.CreateCommand())
            {

                cmd.CommandText = compiler.Compile(query);
                cmd.AddParameterWithValue("assembly_hash", HashHelper.HashMD5(stream));
                cmd.AddParameterWithValue("configuration_hash", configurationHash);
                cmd.AddParameterWithValue("name", name);

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                cmd.AddParameterWithValue("data", buffer);

                cmd.ExecuteNonQuery();
            }
            
        
        }

        private static AssemblyDescription Map(DbDataReader reader)
        {
            return new AssemblyDescription()
            {
                Id = reader.GetInt32(0),
                AssemblyHash = reader.GetString(1),
                ConfigurationHash = reader.GetString(2),
                CreateDataTime = reader.GetDateTime(3),
                Name = reader.GetString(4)
            };
        }

        public static List<AssemblyDescription> GetLastAssemblies(SqlCompillerBase compiler, DataContext context)
        {
            var list = new List<AssemblyDescription>();

            var query = new SelectQueryNode()
                        .From("assemblies")
                        .Select("id")
                        .Select("assembly_hash")
                        .Select("configuration_hash")
                        .Select("create_datetime")
                        .Select("name");

            var cmdText = compiler.Compile(query);
            using (var cmd = context.CreateCommand())
            {
                cmd.CommandText = cmdText;


                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    list.Add(Map(reader));

                }


                return list.GroupBy(a => a.Name).Select(g => g.Aggregate((a1, a2) => a1.Id > a2.Id ? a1 : a2)).ToList();
            }



        }

        public static AssemblyDescription GetLastAssemblyDescriptionByName(string name, SqlCompillerBase compiler, DataContext context)
        {
            var query = new SelectQueryNode()
                        .From("assemblies")
                        .WithTop(1)
                        .Select("id")
                        .Select("assembly_hash")
                        .Select("configuration_hash")
                        .Select("create_datetime")
                        .Select("name")
                        .Where(x => x.Field("name"), "=", x => x.Parameter("name"))
                        .OrderBy("id").Desc();


            var cmdText = compiler.Compile(query);

            using (var cmd = context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        return Map(reader);


                    }
                }
                return null;
            }
        }

        public static Stream GetLastAssemblyByName(string name, SqlCompillerBase compiler, DataContext context)
        {
            var query = new SelectQueryNode()
                        .From("assemblies")
                        .WithTop(1)
                        .Select("data")
                        .Where(x => x.Field("name"), "=", x => x.Parameter("name"))
                        .OrderBy("id").Desc();


            var cmdText = compiler.Compile(query);

            using (var cmd = context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("name", name);


                MemoryStream ms = new MemoryStream((byte[])cmd.ExecuteScalar());
                return ms;
            }
        }

        public static Stream GetAssemblyByDescription(AssemblyDescription description, SqlCompillerBase compiler, DataContext context)
        {
            return GetAssemblyById(description.Id, compiler, context);
        }

        public static Stream GetAssemblyById(int id, SqlCompillerBase compiler, DataContext context)
        {
            var query = new SelectQueryNode()
                        .From("assemblies")
                        .Select("data")
                        .Where(x => x.Field("id"), "=", x => x.Parameter("id"));


            var cmdText = compiler.Compile(query);
            using (var cmd = context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("id", id);


                MemoryStream ms = new MemoryStream((byte[])cmd.ExecuteScalar());
                return ms;
            }
        }


        

        public static void BuildConfiguration(SqlCompillerBase compiler, DataContext context)
        {


            var storage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME,
                context,
                compiler);

            var conf = XCRoot.Load(storage);
            BuildConfiguration(conf, compiler, context);
        }

        public static void BuildConfiguration(XCRoot configuration, SqlCompillerBase compiler, DataContext context)
        {
            XCCompiller compiller = new XCCompiller();

            

            var assemblyFile = compiller.Build(configuration, Path.GetTempPath(), "MainServerAssembly");

            using (FileStream fs = new FileStream(assemblyFile, FileMode.Open))
            {
                SaveAssembly("MainServerAssembly", HashHelper.HashMD5(configuration.SerializeToStream()), fs, compiler, context);
            }


        }

    }
}
