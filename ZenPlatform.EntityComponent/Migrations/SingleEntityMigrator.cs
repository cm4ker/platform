using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ZenPlatform.Configuration.CompareTypes;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Migration;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.EntityComponent.Migrations
{
    

    /*
     * Мигрирование происходит в несколько этапов
     * 1) Создание таблицы для нового типа структуры
     * 2) Перекачивание данных с трансформацией из старого типа структуры в новое
     * 3) Удаление старых структур
     * 4) Переименование новых структур
     */

    public class SingleEntityMigrator : IEntityMigrator
    {
        /// <summary>
        /// Создать новую миграцию для сущности
        /// </summary>
        /// <param name="store">хранилище настроек</param>
        /// <param name="old">Старая конфигурация</param>
        /// <param name="actual">Новая конфигурация</param>
        public SingleEntityMigrator()
        {
        }

        private bool NeedToChangeDatabase(XCSingleEntity x, XCSingleEntity y)
        {
           return !x.Properties.SequenceEqual(y.Properties);
        }

        private void DeleteTableTask(int step, List<IMigrationTask> tasks, string tableName)
        {
            tasks.Add(new MigrationTaskAction(step,
                    query =>
                    {
                        query.Delete().Table(tableName);
                    },
                    query =>
                    {

                    }, $"Delete table {tableName}"));
        }

        private void CopyTableTask(int step, List<IMigrationTask> tasks, string tableNameFrom, string tableNameTo)
        {
            tasks.Add(new MigrationTaskAction(step,
                    query =>
                    {
                        query.Copy().Table().FromTable(tableNameFrom).ToTable(tableNameTo);
                    },
                    query =>
                    {
                        query.Delete().Table(tableNameTo);
                    }, $"Copy table {tableNameFrom} => {tableNameTo}"));
        }

        private void RenameTableTask(int step, List<IMigrationTask> tasks, string tableNameFrom, string tableNameTo)
        {
            tasks.Add(new MigrationTaskAction(step,
                    query =>
                    {
                        query.Rename().Table(tableNameFrom).To(tableNameTo);
                    },
                    query =>
                    {
                        query.Rename().Table(tableNameTo).To(tableNameFrom);
                    }, $"Rename table {tableNameFrom} => {tableNameTo}"));
        }


        private void CreateTableTask(int step, List<IMigrationTask> tasks, XCSingleEntity entity)
        {
            tasks.Add(new MigrationTaskAction(step,
                    query =>
                    {
                        var tableBuilder = query.Create().Table(entity.RelTableName);

                        foreach (var property in entity.Properties)
                        {
                            property.GetPropertySchemas().ForEach(s =>
                            {
                                tableBuilder.WithColumnDefinition(GetColumnDefenitionBySchema(s));
                            });
                        }
                    },
                    query =>
                    {
                        query.Delete().Table(entity.RelTableName);
                    }, $"Create table {entity.RelTableName}"));
        }

        private void ChangeTableTask(int step, List<IMigrationTask> tasks, XCSingleEntity old, XCSingleEntity actual)
        {
            tasks.Add(new MigrationTaskAction(step,
                    query =>
                    {
                        string tableName = $"{actual.RelTableName}_tmp";

                        var props = old.GetProperties()
                           .FullJoin(
                               actual.GetProperties(), x => x.Guid,
                               x => new { old = x, actual = default(IXProperty) },
                               x => new { old = default(IXProperty), actual = x },
                               (x, y) => new { old = x, actual = y });

                        foreach (var property in props)
                        {
                            if (property.old == null)
                            {
                                CreateProperty(query, property.actual, tableName);
                            }
                            else
                            if (property.actual == null)
                            {
                                DeleteProperty(query, property.old, tableName);
                            }
                            else
                            {
                                if (!property.old.Equals(property.actual))
                                    ChangeProperty(query, property.old, property.actual, tableName);
                            }
                        }
                    },
                    query =>
                    {
                        
                    }, $"Change table {actual.RelTableName}_tmp"));
            
        }


        public IList<IMigrationTask> GetMigration(IXCComponent oldState, IXCComponent actualState)
        {
            var oldTypes = oldState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();
            var actualTypes = actualState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();

            var types = oldTypes.FullJoin(actualTypes, x => x.Guid,
                x => new { old = x, actual = default(XCSingleEntity) },
                x => new { old = default(XCSingleEntity), actual = x },
                (x, y) => new { old = x, actual = y });


            var tasks = new List<IMigrationTask>();



            foreach (var entitys in types)
            {
                var old = entitys.old;
                var actual = entitys.actual;

                if (old == null && actual == null)
                {
                    
                }
                else
                if (old != null && actual == null)
                {
                    DeleteTableTask(10, tasks, old.RelTableName);
                    
                }
                else
                if (old == null && actual != null)
                {
                    CreateTableTask(10, tasks, actual);



                }
                else
                {
                    if (NeedToChangeDatabase(old, actual))
                    {
                        CopyTableTask(10, tasks, old.RelTableName, $"{actual.RelTableName}_tmp");
                        ChangeTableTask(20, tasks, old, actual);
                        DeleteTableTask(30, tasks, old.RelTableName);
                        RenameTableTask(40, tasks, $"{actual.RelTableName}_tmp", old.RelTableName);

                    }
                        
                }
            }


            return tasks;
        }


        private void DeleteSchema(DDLQuery query, XCColumnSchemaDefinition schema, string tableName)
        {
            query.Delete().Column(schema.FullName).OnTable(tableName);
        }

        private void ChangeSchema(DDLQuery query, XCColumnSchemaDefinition schema, string tableName)
        {
            query.Alter().Column(GetColumnDefenitionBySchema(schema)).OnTable(tableName);
        }

        private void CreateSchema(DDLQuery query, XCColumnSchemaDefinition schema, string tableName)
        {
            query.Create().Column(GetColumnDefenitionBySchema(schema)).OnTable(tableName);
        }

        public void CreateProperty(DDLQuery query, IXProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => { CreateSchema(query, s, tableName); });
        }

        public void DeleteProperty(DDLQuery query, IXProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => DeleteSchema(query, s, tableName));
        }

        public void ChangeProperty(DDLQuery query, IXProperty old, IXProperty actual, string tableName)
        {
            var schemas = old.GetPropertySchemas()
                            .FullJoin(
                           actual.GetPropertySchemas(),
                           x => x.FullName,
                           x => new { old = x, actual = default(XCColumnSchemaDefinition) },
                           x => new { old = default(XCColumnSchemaDefinition), actual = x },
                           (x, y) => new { old = x, actual = y });

            foreach (var schema in schemas)
            {
                if (schema.old == null)
                {
                    CreateSchema(query, schema.actual, tableName);
                } 
                else if (schema.actual == null)
                {
                    DeleteSchema(query, schema.old, tableName);
                } else
                {
                    if (schema.old.PlatformType is XCObjectTypeBase || schema.actual.PlatformType is XCObjectTypeBase)
                    {
                        if (schema.old.PlatformType.Guid != schema.actual.PlatformType.Guid)
                            ChangeSchema(query, schema.actual, tableName);
                    } else
                    if (!schema.old.PlatformType.Equals(schema.actual.PlatformType))
                        ChangeSchema(query, schema.actual, tableName);
                }
            }
        }

        public ColumnDefinition GetColumnDefenitionBySchema(XCColumnSchemaDefinition schema)
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
            } else
            if (schema.SchemaType == XCColumnSchemaType.Ref)
            {
                builder.AsGuid();
            }


            

            return builder.ColumnDefinition;
        }
        
        

    }
}