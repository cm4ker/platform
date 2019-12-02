using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ZenPlatform.Configuration.CompareTypes;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
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


        public SSyntaxNode GetStep1(IXCObjectType old, IXCObjectType actual, DDLQuery query)
        {
             

            if (old == null && actual == null)
            {

            } else
            if (old != null && actual == null)
            {
                query.Delete().Table(old.RelTableName);
            } else 
            if (old == null && actual != null )
            {
                var tableBuilder = query.Create().Table(actual.RelTableName);

                foreach (var property in actual.GetProperties())
                {
                    property.GetPropertySchemas().ForEach(s =>
                    {
                        tableBuilder.WithColumnDefinition(GetColumnDefenitionBySchema(s));
                    });
                }
                    
                
            }
            else
            {
                var comparer = new XCObjectTypeComparer<IXCObjectType>();
                if (!comparer.Equals(old, actual))
                {
                    query.Copy().Table().FromTable(old.RelTableName).ToTable($"{actual.RelTableName}_tmp");
                }
            }
            

            return query.Expression;
        }

        public SSyntaxNode GetStep2(IXCObjectType old, IXCObjectType actual, DDLQuery query)
        {


            if (old != null && actual != null)
            {


                var comparer = new XCObjectTypeComparer<IXCObjectProperty>();
                string tableName = $"{actual.RelTableName}_tmp";

                var props = old.GetProperties()
                   .FullJoin(
                       actual.GetProperties(), x => x.Guid, 
                       x => new { old = x, actual = default(IXCObjectProperty) },
                       x => new { old = default(IXCObjectProperty), actual = x },
                       (x, y) => new { old = x, actual = y });

                foreach (var property in props)
                {
                    if (property.old == null)
                    {
                        CreateProperty(query, property.actual, tableName);
                    } else
                    if (property.actual == null)
                    {
                        DeleteProperty(query, property.old, tableName);
                    } else
                    {
                        if (!comparer.Equals(property.old, property.actual))
                            ChangeProperty(query, property.old, property.actual, tableName);
                    }
                }
                
            }

            return query.Expression;
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

        public void CreateProperty(DDLQuery query, IXCObjectProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => { CreateSchema(query, s, tableName); });
        }

        public void DeleteProperty(DDLQuery query, IXCObjectProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => DeleteSchema(query, s, tableName));
        }

        public void ChangeProperty(DDLQuery query, IXCObjectProperty old, IXCObjectProperty actual, string tableName)
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


        public SSyntaxNode GetStep3(IXCObjectType old, IXCObjectType actual, DDLQuery query)
        {
            var comparer = new XCObjectTypeComparer<IXCObjectType>();
            if (old != null && actual != null && !comparer.Equals(old, actual))
            {
                query.Delete().Table($"{actual.RelTableName}");
            }
            return query.Expression;
        }

        public SSyntaxNode GetStep4(IXCObjectType old, IXCObjectType actual, DDLQuery query)
        {
            var comparer = new XCObjectTypeComparer<IXCObjectType>();
            if (old != null && actual != null && !comparer.Equals(old,actual))
            {
                query.Rename().Table($"{actual.RelTableName}_tmp").To(old.RelTableName);
            }
            return query.Expression;
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
                    case XCObjectTypeBase t:
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