using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.Core.Assemblies
{
    public class AssemblyManager : IAssemblyManager
    {
        private IDataContextManager _dataContextManager;
        private IXCCompiller _compiller;

        public AssemblyManager(IDataContextManager dataContextManager, IXCCompiller compiller)
        {
            _dataContextManager = dataContextManager;
            _compiller = compiller;
        }

        public void BuildConfiguration(XCRoot configuration)
        {

            var assemblies = _compiller.Build(configuration);

            foreach (var assemblyStream in assemblies)
            {
                SaveAssembly(assemblyStream.Key, HashHelper.HashMD5(configuration.SerializeToStream()), assemblyStream.Value);
            }


        }
        public void SaveAssembly(string name, string configurationHash, Stream stream)
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

            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {

                cmd.CommandText = _dataContextManager.SqlCompiler.Compile(query);
                cmd.AddParameterWithValue("assembly_hash", HashHelper.HashMD5(stream));
                cmd.AddParameterWithValue("configuration_hash", configurationHash);
                cmd.AddParameterWithValue("name", name);

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                cmd.AddParameterWithValue("data", buffer);

                cmd.ExecuteNonQuery();
            }


        }

        private AssemblyDescription Map(DbDataReader reader)
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

        public List<AssemblyDescription> GetLastAssemblies()
        {
            var list = new List<AssemblyDescription>();

            var query = new SelectQueryNode()
                        .From("assemblies")
                        .Select("id")
                        .Select("assembly_hash")
                        .Select("configuration_hash")
                        .Select("create_datetime")
                        .Select("name");

            var cmdText = _dataContextManager.SqlCompiler.Compile(query);
            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;


                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));

                    }
                }

                return list.GroupBy(a => a.Name).Select(g => g.Aggregate((a1, a2) => a1.Id > a2.Id ? a1 : a2)).ToList();
            }



        }

        public AssemblyDescription GetLastAssemblyDescriptionByName(string name)
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


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);

            using (var cmd = _dataContextManager.GetContext().CreateCommand())
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

        public Stream GetLastAssemblyByName(string name)
        {
            var query = new SelectQueryNode()
                        .From("assemblies")
                        .WithTop(1)
                        .Select("data")
                        .Where(x => x.Field("name"), "=", x => x.Parameter("name"))
                        .OrderBy("id").Desc();


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);

            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("name", name);


                MemoryStream ms = new MemoryStream((byte[])cmd.ExecuteScalar());
                return ms;
            }
        }

        public Stream GetAssemblyByDescription(AssemblyDescription description)
        {
            return GetAssemblyById(description.Id);
        }

        public Stream GetAssemblyById(int id)
        {
            var query = new SelectQueryNode()
                        .From("assemblies")
                        .Select("data")
                        .Where(x => x.Field("id"), "=", x => x.Parameter("id"));


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);
            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("id", id);


                MemoryStream ms = new MemoryStream((byte[])cmd.ExecuteScalar());
                return ms;
            }
        }

    }
}
