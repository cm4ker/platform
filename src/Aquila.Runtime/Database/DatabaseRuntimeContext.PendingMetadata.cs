using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using Aquila.Data;
using Aquila.Metadata;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aquila.Runtime
{
    /// <summary>
    /// Database context for already running server
    /// </summary>
    public partial class DatabaseRuntimeContext
    {
        private const string PendingMetadataTableName = "metadata_pending";
        private EntityMetadataCollection _pendingMd;


        /// <summary>
        /// Returns metadata model for current database (from moment then DatabaseRuntimeContext being loaded) state
        /// </summary>
        /// <returns></returns>
        public EntityMetadataCollection GetPendingMetadata()
        {
            return _pendingMd;
        }


        #region Metadata

        private void LoadPendingMetadata(DataConnectionContext context)
        {
            using var cmd = context.CreateCommand(q =>
            {
                q.bg_query()
                    .m_from()
                    .ld_table(PendingMetadataTableName)
                    .m_select()
                    .ld_column("blob_name")
                    .ld_column("data")
                    .st_query();
            });

            using var reader = cmd.ExecuteReader();

            var list = new List<EntityMetadata>();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            while (reader.Read())
            {
                var data = (byte[])reader["data"];

                var yaml = Encoding.UTF8.GetString(data);
                var md = deserializer.Deserialize<EntityMetadata>(yaml);

                list.Add(md);
            }

            _pendingMd = new EntityMetadataCollection(list);
        }

        private void SavePendingMetadata(DataConnectionContext context)
        {
            using var cmd = context.CreateCommand(q =>
            {
                q.bg_query()
                    .m_values()
                    .ld_param("blob_name")
                    .ld_param("data")
                    .m_insert()
                    .ld_table(PendingMetadataTableName)
                    .ld_column("blob_name")
                    .ld_column("data")
                    .st_query();
            });

            foreach (var md in _pendingMd)
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(md);

                //TODO: remove this static
                cmd.AddOrSetParameterWithValue("blob_name", $"Entity.{md.Name}");
                cmd.AddOrSetParameterWithValue("data", Encoding.UTF8.GetBytes(yaml));

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}