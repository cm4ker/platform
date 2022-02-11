using System.Collections.Generic;
using System.Text;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.QueryBuilder;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DBConsts = Aquila.Initializer.DBConstNames.Metadata;

namespace Aquila.Runtime
{
    public partial class DatabaseRuntimeContext
    {
        public class MetadataRC
        {
            private readonly string _tableName;
            protected MetadataProvider _md;

            public MetadataRC(string tableName)
            {
                _tableName = tableName;
                _md = new MetadataProvider();
            }

            /// <summary>
            /// Returns metadata model for current database (from moment then DatabaseRuntimeContext being loaded) state
            /// </summary>
            /// <returns></returns>
            public MetadataProvider GetMetadata()
            {
                //return TestMetadata.GetTestMetadata();
                return _md;
            }

            #region Metadata

            public void LoadMetadata(DataConnectionContext context)
            {
                using var cmd = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .m_select()
                        .ld_column(DBConsts.BLOB_NAME_COLUMN)
                        .ld_column(DBConsts.DATA_COLUMN)
                        .st_query();
                });

                using var reader = cmd.ExecuteReader();

                var list = new List<EntityMetadata>();
                var secList = new List<SecPolicyMetadata>();

                while (reader.Read())
                {
                    var name = (string)reader[DBConsts.BLOB_NAME_COLUMN];
                    var data = (byte[])reader[DBConsts.DATA_COLUMN];

                    if (name.StartsWith("Entity"))
                    {
                        var yaml = Encoding.UTF8.GetString(data);
                        var md = EntityMetadata.FromYaml(yaml);
                        list.Add(md);
                    }
                    else if (name.StartsWith("Sec"))
                    {
                        var yaml = Encoding.UTF8.GetString(data);
                        var md = SecPolicyMetadata.FromYaml(yaml);
                        secList.Add(md);
                    }
                }

                _md = new MetadataProvider(list, secList);
            }

            public void SaveMetadata(DataConnectionContext context)
            {
                context.BeginTransaction();

                context.CreateCommand(q =>
                {
                    q.bg_query()
                        .ld_table(_tableName)
                        .@as("T0")
                        .m_from()
                        .m_delete()
                        .ld_table("T0")
                        .st_query();
                }).ExecuteNonQuery();

                using var cmd = context.CreateCommand(q =>
                {
                    q.bg_query()
                        .m_values()
                        .ld_param(DBConsts.BLOB_NAME_COLUMN)
                        .ld_param(DBConsts.DATA_COLUMN)
                        .m_insert()
                        .ld_table(_tableName)
                        .ld_column(DBConsts.BLOB_NAME_COLUMN)
                        .ld_column(DBConsts.DATA_COLUMN)
                        .st_query();
                });

                foreach (var md in _md)
                {
                    var yaml = EntityMetadata.ToYaml(md);

                    //TODO: remove this static
                    cmd.AddOrSetParameterWithValue(DBConsts.BLOB_NAME_COLUMN, $"Entity.{md.Name}");
                    cmd.AddOrSetParameterWithValue(DBConsts.DATA_COLUMN, Encoding.UTF8.GetBytes(yaml));

                    cmd.ExecuteNonQuery();
                }

                foreach (var in)
                {
                }

                context.CommitTransaction();
            }

            public void TransferTo(DataConnectionContext dcc, MetadataRC destenation)
            {
                dcc.BeginTransaction();

                destenation.Clear(dcc);
                dcc.CreateCommand((qm) =>
                    qm.bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .@as("t1")
                        .m_select()
                        .ld_column("*", "t1")
                        .m_insert()
                        .ld_table(destenation._tableName)
                        .st_query()).ExecuteNonQuery();

                dcc.CommitTransaction();
            }

            public void SetMetadata(MetadataProvider md)
            {
                _md = md;
            }

            public void Clear(DataConnectionContext dcc)
            {
                _md = new MetadataProvider();

                void Gen(QueryMachine qm)
                {
                    qm
                        .bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .@as("T0")
                        .m_where()
                        .ld_const("1")
                        .ld_const("1")
                        .eq()
                        .m_delete()
                        .ld_table("T0")
                        .st_query();
                }

                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            #endregion
        }
    }
}