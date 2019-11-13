using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Assemlies
{
    public class DatabaseAssemblyStorage : IAssemblyStorage
    {
        private IDataContextManager _dataContextManager;
        public DatabaseAssemblyStorage(IDataContextManager dataContextManager)
        {
            _dataContextManager = dataContextManager;
        }

        private AssemblyDescription Map(DbDataReader reader)
        {
            return new AssemblyDescription()
            {
                AssemblyHash = reader.GetString(0),
                ConfigurationHash = reader.GetString(1),
                Type = (Compiler.AssemblyType)reader.GetInt32(2),
                Name = reader.GetString(3)
            };
        }

        public byte[] GetAssembly(string configurationHash, string name)
        {
            var query = new SelectQueryNode()
                      .From("assemblies")
                      .Select("data")
                      .Where(x => x.Field("name"), "=", x => x.Parameter("name"))
                      .Where(x => x.Field("configuration_hash"), "=", x => x.Parameter("configuration_hash"));


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);
            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("name", name);
                cmd.AddParameterWithValue("configuration_hash", configurationHash);

                return (byte[])cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(string assemblyHash)
        {
            var query = new SelectQueryNode()
                       .From("assemblies")
                       .Select("data")
                       .Where(x => x.Field("assembly_hash"), "=", x => x.Parameter("assembly_hash"));


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);
            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("assembly_hash", assemblyHash);


                return (byte[])cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(AssemblyDescription description)
        {
            return GetAssembly(description.AssemblyHash);
        }


        public IEnumerable<AssemblyDescription> GetAssemblies(string configurationHash)
        {
            var list = new List<AssemblyDescription>();

            var query = new SelectQueryNode()
                        .From("assemblies")
                        .Select("assembly_hash")
                        .Select("configuration_hash")
                        .Select("type")
                        .Select("name")
                        .Where(x => x.Field("configuration_hash"), "=", x => x.Parameter("configuration_hash"));


            var cmdText = _dataContextManager.SqlCompiler.Compile(query);
            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("configuration_hash", configurationHash);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));

                    }
                }

                return list;
            }
            
        }

        public void SaveAssembly(AssemblyDescription description, byte[] blob)
        {


            var query = new InsertQueryNode()
               .InsertInto("assemblies")
               .WithFieldAndValue(x => x.Field("assembly_hash"),
                   x => x.Parameter("assembly_hash"))
               .WithFieldAndValue(x => x.Field("configuration_hash"),
                   x => x.Parameter("configuration_hash"))
               .WithFieldAndValue(x => x.Field("type"),
                   x => x.Parameter("type"))
               .WithFieldAndValue(x => x.Field("name"),
                   x => x.Parameter("name"))
               .WithFieldAndValue(x => x.Field("data"),
                   x => x.Parameter("data"));

            using (var cmd = _dataContextManager.GetContext().CreateCommand())
            {

                cmd.CommandText = _dataContextManager.SqlCompiler.Compile(query);
                cmd.AddParameterWithValue("assembly_hash", description.AssemblyHash);
                cmd.AddParameterWithValue("configuration_hash", description.ConfigurationHash);
                cmd.AddParameterWithValue("name", description.Name);
                cmd.AddParameterWithValue("type", description.Type);


                cmd.AddParameterWithValue("data", blob);

                cmd.ExecuteNonQuery();
            }
        }


    }
}
