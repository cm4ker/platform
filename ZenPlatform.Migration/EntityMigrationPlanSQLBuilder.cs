﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Migration;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Migration
{
    public class EntityMigrationPlanSQLBuilder : IEntityMigrationPlanBuilder
    {
        #region Visit

        private Guid _migration_id;

        public EntityMigrationPlanSQLBuilder(Guid migration_id)
        {
            _migration_id = migration_id;
        }
        public void AddColumnItemVisit(DDLQuery query, AddColumnItem item)
        {
            query.Delete().Table(item.TableName).IfExists();
            query.Create().Column(GetColumnDefenitionBySchema(item.Schema)).OnTable(item.TableName);
        }
        public void AlterColumnItemVisit(DDLQuery query, AlterColumnItem item)
        {
            query.Alter().Column(GetColumnDefenitionBySchema(item.Schema)).OnTable(item.TableName);
        }
        public void DeleteColumnItemVisit(DDLQuery query, DeleteColumnItem item)
        {
            query.Delete().Column(item.Schema.FullName).OnTable(item.TableName);
        }
        public void UpdateTypeItemVisit(DDLQuery query, UpdateTypeItem item)
        {
            var type = item.Property.GetPropertySchemas().First(s => s.SchemaType == XCColumnSchemaType.Type);
            query.Add(m =>
            {
                m
                .bg_query()

                .m_set()
                    .ld_column(type.FullName)
                    .ld_const(item.XCType.Id)
                    .assign()
                .m_update()
                    .ld_table(item.TableName)
                .st_query()
                ;
            });
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
                tableBuilder.WithColumnDefinition(GetColumnDefenitionBySchema(schema));

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

        private ColumnDefinition GetColumnDefenitionBySchema(XCColumnSchemaDefinition schema)
        {

            ColumnDefinitionBuilder builder = new ColumnDefinitionBuilder();
            builder.WithColumnName(schema.FullName);

            if (schema.SchemaType == XCColumnSchemaType.Value

                 || schema.SchemaType == XCColumnSchemaType.NoSpecial)
            {

                var type = schema.PlatformType;

                switch (type)
                {
                    case XCBoolean t:
                        builder.AsBoolean().NotNullable();
                        break;
                    case XCString t:
                        builder.AsString(t.Size).NotNullable();
                        break;
                    case XCDateTime t:
                        builder.AsDateTime().NotNullable();
                        break;
                    case XCGuid t:
                        builder.AsGuid().NotNullable();
                        break;
                    case XCNumeric t:
                        builder.AsFloat(t.Scale, t.Precision).NotNullable();
                        break;
                    case XCBinary t:
                        builder.AsVarBinary(t.Size).NotNullable();
                        break;
                    case XCInt t:
                        builder.AsInt().NotNullable();
                        break;
                    case IXCStructureType t:
                        builder.AsGuid().NotNullable();
                        break;
                }
            }
            else
            if (schema.SchemaType == XCColumnSchemaType.Type)
            {
                builder.AsInt().Nullable();
            }
            else
            if (schema.SchemaType == XCColumnSchemaType.Ref)
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
