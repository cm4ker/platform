using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Metadata;
using DBConsts = Aquila.Initializer.DBConstNames.Descriptors;

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
                        .ld_param(DBConsts.MD_NAME_COLUMN)
                        .ld_param(DBConsts.DB_NAME_COLUMN)
                        .m_insert()
                        .ld_table(DBConsts.DESCRIPTORS_TABLE)
                        .ld_column(DBConsts.DB_ID_COLUMN)
                        .ld_column(DBConsts.MD_NAME_COLUMN)
                        .ld_column(DBConsts.DB_NAME_COLUMN)
                        .st_query();
                });


                cmd.AddOrSetParameterWithValue(DBConsts.DB_NAME_COLUMN, "Unknown");
                cmd.AddOrSetParameterWithValue(DBConsts.MD_NAME_COLUMN, mdId);

                cmd.ExecuteNonQuery();

                using var cmdLoad = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_from()
                        .ld_table(DBConsts.DESCRIPTORS_TABLE)
                        .m_where()
                        .ld_column(DBConsts.MD_NAME_COLUMN)
                        .ld_param(DBConsts.MD_NAME_COLUMN)
                        .eq()
                        .m_select()
                        .ld_column("*")
                        .st_query();
                });

                cmdLoad.AddOrSetParameterWithValue(DBConsts.MD_NAME_COLUMN, mdId);
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

            /// <summary>
            /// Contains translation from objects to the database
            /// </summary>
            public IEnumerable<EntityDescriptor> GetEntityDescriptors()
            {
                return _descriptors.AsReadOnly();
            }

            public EntityDescriptor GetEntityDescriptor(string key)
            {
                return _descriptors.FirstOrDefault(x => x.MetadataId == key);
            }

            public EntityDescriptor GetEntityDescriptor(int typeId)
            {
                return _descriptors.FirstOrDefault(x => x.DatabaseId == typeId);
            }

            public void SaveDescriptors(DataConnectionContext context)
            {
                using var cmd = context.CreateCommand(q =>
                {
                    q
                        .bg_query()
                        .m_from()
                        .ld_table(DBConsts.DESCRIPTORS_TABLE)
                        .@as("t")
                        .m_where()
                        .ld_column(DBConsts.ID_COLUMN)
                        .ld_param(DBConsts.ID_COLUMN)
                        .eq()
                        .m_set()
                        .ld_column(DBConsts.MD_NAME_COLUMN)
                        .ld_param(DBConsts.MD_NAME_COLUMN)
                        .assign()
                        .ld_column(DBConsts.DB_ID_COLUMN)
                        .ld_param(DBConsts.DB_ID_COLUMN)
                        .assign()
                        .ld_column(DBConsts.DB_NAME_COLUMN)
                        .ld_param(DBConsts.DB_NAME_COLUMN)
                        .assign()
                        .m_update()
                        .ld_table("t")
                        .st_query();
                });

                foreach (var descriptor in _descriptors)
                {
                    cmd.AddOrSetParameterWithValue(DBConsts.DB_NAME_COLUMN, descriptor.DatabaseName);
                    cmd.AddOrSetParameterWithValue(DBConsts.MD_NAME_COLUMN, descriptor.MetadataId);
                    cmd.AddOrSetParameterWithValue(DBConsts.DB_ID_COLUMN, descriptor.DatabaseId);
                    cmd.AddOrSetParameterWithValue(DBConsts.ID_COLUMN, descriptor.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            public void LoadDescriptors(DataConnectionContext context)
            {
                using var cmd = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_from()
                        .ld_table(DBConsts.DESCRIPTORS_TABLE)
                        .m_select()
                        .ld_column(DBConsts.ID_COLUMN)
                        .ld_column(DBConsts.MD_NAME_COLUMN)
                        .ld_column(DBConsts.DB_NAME_COLUMN)
                        .ld_column(DBConsts.DB_ID_COLUMN)
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