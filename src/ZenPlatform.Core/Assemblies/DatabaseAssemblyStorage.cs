using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;

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
                Type = (Compiler.AssemblyType) reader.GetInt32(2),
                Name = reader.GetString(3)
            };
        }

        public byte[] GetAssembly(string configurationHash, string name)
        {
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_from()
                    .ld_table("assemblies")
                    .m_where()
                    .ld_column("name")
                    .ld_param("name")
                    .eq()
                    .ld_column("configuration_hash")
                    .ld_param("configuration_hash")
                    .eq()
                    .and()
                    .m_select()
                    .ld_column("data")
                    .st_query();
            }


            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("name", name);
                cmd.AddParameterWithValue("configuration_hash", configurationHash);

                return (byte[]) cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(string assemblyHash)
        {
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_from()
                    .ld_table("assemblies")
                    .m_where()
                    .ld_column("assembly_hash")
                    .ld_param("assembly_hash")
                    .eq()
                    .and()
                    .m_select()
                    .ld_column("data")
                    .st_query();
            }


            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("assembly_hash", assemblyHash);


                return (byte[]) cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(AssemblyDescription description)
        {
            return GetAssembly(description.AssemblyHash);
        }


        public IEnumerable<AssemblyDescription> GetAssemblies(string configurationHash)
        {
            var list = new List<AssemblyDescription>();


            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_from()
                    .ld_table("assemblies")
                    .m_where()
                    .ld_column("configuration_hash")
                    .ld_param("configuration_hash")
                    .eq()
                    //.and()
                    .m_select()
                    .ld_column("name")
                    .ld_column("type")

                    .ld_column("configuration_hash")
                    .ld_column("assembly_hash")
                    
                    
                    .st_query();
            }


            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
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
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_values()
                        .ld_param("configuration_hash")
                        .ld_param("assembly_hash")
                        .ld_param("type")
                        .ld_param("name")
                        .ld_param("data")
                    .m_insert()
                        .ld_table("assemblies")
                        .ld_column("configuration_hash")
                        .ld_column("assembly_hash")
                        .ld_column("type")
                        .ld_column("name")
                        .ld_column("data")
                    .st_query();

           }

            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
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