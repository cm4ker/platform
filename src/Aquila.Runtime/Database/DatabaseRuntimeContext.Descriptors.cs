using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Data;
using Aquila.Metadata;

namespace Aquila.Runtime
{
    public partial class DatabaseRuntimeContext
    {
        public class DescriptorsRC
        {
            private List<EntityDescriptor> _descriptors;

            public DescriptorsRC()
            {
                _descriptors = new List<EntityDescriptor>();
            }

            /// <summary>
            /// Contains translation from objects to the database
            /// </summary>
            public IEnumerable<EntityDescriptor> GetEntityDescriptors()
            {
                return _descriptors.AsReadOnly();
            }

            public EntityDescriptor GetEntityDescriptor(string s_id)
            {
                return _descriptors.FirstOrDefault(x => x.MetadataId == s_id);
            }


            #region Descriptors

            /// <summary>
            /// Get random string for new metadata. Because we don't know the DatabaseId (it not assigned yet)
            /// </summary>
            /// <returns></returns>
            private string GetRandom()
            {
                Random ran = new Random();

                String b = "abcdefghijklmnopqrstuvwxyz";

                int length = 10;

                String random = "";

                for (int i = 0; i < length; i++)
                {
                    int a = ran.Next(26);
                    random = random + b.ElementAt(a);
                }

                return random;
            }

            /// <summary>
            /// Create new descriptor for certain database context
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public EntityDescriptor CreateDescriptor(DataConnectionContext context)
            {
                var mdId = GetRandom();

                using var cmd = context.CreateCommand(q =>
                {
                    q
                        .bg_query()
                        .m_values()
                        .ld_const(0)
                        .ld_param("id_s")
                        .ld_param("db_name")
                        .m_insert()
                        .ld_table(DescriptorsTableName)
                        .ld_column("id_n")
                        .ld_column("id_s")
                        .ld_column("db_name")
                        .st_query();
                });


                cmd.AddOrSetParameterWithValue("db_name", "Unknown");
                cmd.AddOrSetParameterWithValue("id_s", mdId);

                cmd.ExecuteNonQuery();

                using var cmdLoad = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_from()
                        .ld_table(DescriptorsTableName)
                        .m_where()
                        .ld_column("id_s")
                        .ld_param("id_s")
                        .eq()
                        .m_select()
                        .ld_column("*")
                        .st_query();
                });

                cmdLoad.AddOrSetParameterWithValue("id_s", mdId);
                using var reader = cmdLoad.ExecuteReader();

                if (reader.Read())
                {
                    var id = reader.GetInt32(0);

                    var ed = new EntityDescriptor(id);
                    _descriptors.Add(ed);
                    return ed;
                }
                else
                    throw new Exception("Metadata not found");
            }


            public EntityDescriptor FindEntityDescriptor(string key)
            {
                return _descriptors.FirstOrDefault(x => x.MetadataId == key);
            }


            public void SaveDescriptors(DataConnectionContext context)
            {
                using var cmd = context.CreateCommand(q =>
                {
                    q
                        .bg_query()
                        .m_from()
                        .ld_table(DescriptorsTableName)
                        .@as("t")
                        .m_where()
                        .ld_column("id")
                        .ld_param("id")
                        .eq()
                        .m_set()
                        .ld_column("id_s")
                        .ld_param("id_s")
                        .assign()
                        .ld_column("id_n")
                        .ld_param("id_n")
                        .assign()
                        .ld_column("db_name")
                        .ld_param("db_name")
                        .assign()
                        .m_update()
                        .ld_table("t")
                        .st_query();
                });

                foreach (var descriptor in _descriptors)
                {
                    cmd.AddOrSetParameterWithValue("db_name", descriptor.DatabaseName);
                    cmd.AddOrSetParameterWithValue("id_s", descriptor.MetadataId);
                    cmd.AddOrSetParameterWithValue("id_n", descriptor.DatabaseId);
                    cmd.AddOrSetParameterWithValue("id", descriptor.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            public void LoadDescriptors(DataConnectionContext context)
            {
                using var cmd = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_from()
                        .ld_table(DescriptorsTableName)
                        .m_select()
                        .ld_column("id")
                        .ld_column("id_s")
                        .ld_column("db_name")
                        .ld_column("id_n")
                        .st_query();
                });

                using var reader = cmd.ExecuteReader();

                _descriptors.Clear();

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var mId = reader.GetString(1);
                    var dbName = reader.GetString(2);
                    var dbId = reader.GetInt32(3);

                    _descriptors.Add(new EntityDescriptor(id, dbId)
                        { DatabaseName = dbName, MetadataId = mId });
                }
            }

            /// <summary>
            /// Remove descriptor from database and in-memory model
            /// </summary>
            /// <param name="key"></param>
            public void RemoveDescriptor(string key)
            {
            }

            #endregion
        }
    }
}