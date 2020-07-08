using System;
using System.Collections.Generic;
using Aquila.Metadata;
using Aquila.QueryBuilder.Builders;
using Aquila.QueryBuilder.Model;

namespace Aquila.Migrations
{
    public class EntityMigrationPlanBuilder
    {
        #region Visit

        private Guid _migration_id;

        public EntityMigrationPlanBuilder(Guid migration_id)
        {
            _migration_id = migration_id;
        }

        public void AddColumnItemVisit(DDLQuery query, AddColumnItem item)
        {
            query.Create().Column(GetColumnDefinitionByProperty(item.Schema)).OnTable(item.TableName);
        }

        public void AlterColumnItemVisit(DDLQuery query, AlterColumnItem item)
        {
            query.Alter().Column(GetColumnDefinitionByProperty(item.Schema)).OnTable(item.TableName);
        }

        public void DeleteColumnItemVisit(DDLQuery query, DeleteColumnItem item)
        {
            query.Delete().Column(item.Schema.FullName).OnTable(item.TableName);
        }

        public void UpdateTypeItemVisit(DDLQuery query, UpdateTypeItem item)
        {
            // var type = item.Property.GetDbSchema().First(s => s.SchemaType == ColumnSchemaType.Type);
            // query.Add(m =>
            // {
            //     m
            //         .bg_query()
            //         .m_set()
            //         .ld_column(type.FullName)
            //         .ld_const(item.XCIpType.Id)
            //         .assign()
            //         .m_update()
            //         .ld_table(item.TableName)
            //         .st_query()
            //         ;
            // });
        }

        public void RenameColumnItemVisit(DDLQuery query, RenameColumnItem item)
        {
            query.Rename().Column(item.TableName).From(item.SrcSchema.FullName).To(item.DstSchema.FullName);
        }

        public void CopyTableItemVisit(DDLQuery query, CopyTableItem item)
        {
            query.Delete().Table(item.DstTableName).IfExists();
            query.Copy().Table().FromTable(item.SrcTableName).ToTable(item.DstTableName);
        }

        public void DeleteTableItemVisit(DDLQuery query, DeleteTableItem item)
        {
            query.Delete().Table(item.TableName);
        }

        public void RenameTableItemVisit(DDLQuery query, RenameTableItem item)
        {
            query.Rename().Table(item.SrcTableName).To(item.DstTableName);
        }

        public void CreateTableItemVisit(DDLQuery query, CreateTableItem item)
        {
            query.Delete().Table(item.TableName).IfExists();
            var tableBuilder = query.Create().Table(item.TableName);

            foreach (var schema in item.SchemaDefinitions)
            {
                tableBuilder.WithColumnDefinition(GetColumnDefinitionByProperty(schema));
            }
        }

        public void SetFlagRenameTableItemVisit(DDLQuery query, SetFlagRenameTableItem item)
        {
            query.Add(m =>
            {
                m
                    .bg_query()
                    .m_where()
                    .ld_column("temp_table")
                    .ld_const(item.TableName)
                    .eq()
                    .m_set()
                    .ld_column("rename_table")
                    .ld_const(true)
                    .assign()
                    .m_update()
                    .ld_table("migration_status")
                    .st_query()
                    ;
            });
        }

        public void SetFlagDeleteTableItemVisit(DDLQuery query, SetFlagDeleteTableItem item)
        {
            query.Add(m =>
            {
                m
                    .bg_query()
                    .m_where()
                    .ld_column("original_table")
                    .ld_const(item.TableName)
                    .eq()
                    .m_set()
                    .ld_column("delete_table")
                    .ld_const(true)
                    .assign()
                    .m_update()
                    .ld_table("migration_status")
                    .st_query()
                    ;
            });
        }

        public void SetFlagChangeTableItemVisit(DDLQuery query, SetFlagChangeTableItem item)
        {
            query.Add(m =>
            {
                m
                    .bg_query()
                    .m_where()
                    .ld_column("temp_table")
                    .ld_const(item.TableName)
                    .eq()
                    .m_set()
                    .ld_column("change_table")
                    .ld_const(true)
                    .assign()
                    .m_update()
                    .ld_table("migration_status")
                    .st_query()
                    ;
            });
        }

        public void SetFlagCopyTableItemVisit(DDLQuery query, SetFlagCopyTableItem item)
        {
            query.Add(m =>
            {
                m
                    .bg_query()
                    .m_values()
                    .ld_const(true)
                    .ld_const(item.SrcTableName)
                    .ld_const(item.DstTableName)
                    .ld_const(_migration_id)
                    .m_insert()
                    .ld_table("migration_status")
                    .ld_column("copy_table")
                    .ld_column("original_table")
                    .ld_column("temp_table")
                    .ld_column("migration_id")
                    .st_query();
            });
        }

        #endregion

        private ColumnDefinition GetColumnDefinitionByProperty(ColumnSchemaDefinition schema)
        {
            ColumnDefinitionBuilder builder = new ColumnDefinitionBuilder();
            builder.WithColumnName(schema.FullName);

            if (schema.SchemaType == ColumnSchemaType.Value
                || schema.SchemaType == ColumnSchemaType.NoSpecial)
            {
                var type = schema.Type;

                switch (type.Kind)
                {
                    case SMTypeKind.Bool:
                        builder.AsBoolean().NotNullable();
                        break;
                    case SMTypeKind.String:
                        builder.AsString(type.Size).NotNullable();
                        break;
                    case SMTypeKind.DateTime:
                        builder.AsDateTime().NotNullable();
                        break;
                    case SMTypeKind.Guid:
                        builder.AsGuid().NotNullable();
                        break;
                    case SMTypeKind.Numeric:
                        builder.AsFloat(type.Scale, type.Precision).NotNullable();
                        break;
                    case SMTypeKind.Binary:
                        builder.AsVarBinary(type.Size).NotNullable();
                        break;
                    case SMTypeKind.Int:
                        builder.AsInt().NotNullable();
                        break;
                    case SMTypeKind.Reference:
                        builder.AsGuid().NotNullable();
                        break;

                    default: throw new Exception("not supported type");
                }
            }
            else if (schema.SchemaType == ColumnSchemaType.Type)
            {
                builder.AsInt().Nullable();
            }
            else if (schema.SchemaType == ColumnSchemaType.Ref)
            {
                builder.AsGuid();
            }

            return builder.ColumnDefinition;
        }


        public void Build(IEnumerable<IEntityMigrationItem> plan, DDLQuery query)
        {
            foreach (var item in plan)
            {
                item.Visit(this, query);
            }
        }
    }
}