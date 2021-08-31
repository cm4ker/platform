using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Assemlies;
using Aquila.Data;
using Aquila.QueryBuilder;

namespace Aquila.Runtime
{
    public partial class DatabaseRuntimeContext
    {
        private AssemblyDescriptor Map(DbDataReader reader)
        {
            return new AssemblyDescriptor()
            {
                AssemblyHash = reader.GetString(0),
                ConfigurationHash = reader.GetString(1),
                Type = (AssemblyType)reader.GetInt32(2),
                Name = reader.GetString(3)
            };
        }

        public byte[] GetAssembly(DataConnectionContext dcc, string configurationHash, string name)
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


            using (var cmd = dcc.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("name", name);
                cmd.AddParameterWithValue("configuration_hash", configurationHash);

                return (byte[])cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(DataConnectionContext dcc, string assemblyHash)
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


            using (var cmd = dcc.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("assembly_hash", assemblyHash);


                return (byte[])cmd.ExecuteScalar();
            }
        }

        public byte[] GetAssembly(DataConnectionContext dcc, AssemblyDescriptor descriptor)
        {
            return GetAssembly(dcc, descriptor.AssemblyHash);
        }

        public byte[] GetLastAssembly(DataConnectionContext dcc)
        {
            var desc = GetAsmDescriptors(dcc, "Hash").FirstOrDefault();
            return GetAssembly(dcc, desc);
        }

        public void Clear(DataConnectionContext dcc)
        {
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_from()
                    .ld_table("assemblies")
                    .m_where()
                    .ld_const("1")
                    .ld_const("1")
                    .eq()
                    .m_delete()
                    .st_query();
            }

            using (var cmd = dcc.CreateCommand(Gen))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<AssemblyDescriptor> GetAsmDescriptors(DataConnectionContext dcc, string configurationHash)
        {
            var list = new List<AssemblyDescriptor>();


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
                    .ld_column("assembly_hash")
                    .ld_column("configuration_hash")
                    .ld_column("type")
                    .ld_column("name")
                                        
                    
                    .st_query();
            }


            using (var cmd = dcc.CreateCommand(Gen))
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

        public void SaveAssembly(DataConnectionContext dcc, AssemblyDescriptor descriptor, byte[] blob)
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

            using (var cmd = dcc.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("assembly_hash", descriptor.AssemblyHash);
                cmd.AddParameterWithValue("configuration_hash", descriptor.ConfigurationHash);
                cmd.AddParameterWithValue("name", descriptor.Name);
                cmd.AddParameterWithValue("type", descriptor.Type);


                cmd.AddParameterWithValue("data", blob);

                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveAssembly(DataConnectionContext dcc, string hash)
        {
            throw new NotImplementedException();
        }
    }
}