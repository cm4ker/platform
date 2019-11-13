using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;

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

/*
        public IExpression GetStep1(XCObjectTypeBase oldBase, XCObjectTypeBase actualBase)
        {
            XCSingleEntity old = (XCSingleEntity)oldBase;
            XCSingleEntity actual = (XCSingleEntity)actualBase;

            DDLQuery query = new DDLQuery();

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

                foreach (var property in actual.Properties)
                {
                    GetColumnDefenition(property).ForEach(c =>
                    {
                        tableBuilder.WithColumnDefinition(c);
                    });
                }
                    
                
            }
            else
            query.Copy().Table().FromTable(old.RelTableName).ToTable($"{actual.RelTableName}_tmp");

            return query;
        }

        public IExpression GetStep2(XCObjectTypeBase oldBase, XCObjectTypeBase actualBase)
        {
            XCSingleEntity old = (XCSingleEntity)oldBase;
            XCSingleEntity actual = (XCSingleEntity)actualBase;

            DDLQuery query = new DDLQuery();

            if (old != null && actual != null)
            {

                var props = old.Properties
                   .FullJoin(
                       actual.Properties, x => x.Guid, x => new { old = x, actual = default(XCSingleEntityProperty) },
                       x => new { old = default(XCSingleEntityProperty), actual = x },
                       (x, y) => new { old = x, actual = y });

                string tableName = $"{actual.RelTableName}_tmp";

                foreach (var property in props)
                {
                    if (property.actual == null)
                    {
                        foreach (var s in property.old.GetPropertySchemas())
                        {
                            query.Delete().Column(s.Name).OnTable(tableName);
                        }

                    }
                    else if (property.old == null)
                    {
                        GetColumnDefenition(property.actual).ForEach(c => query.Alter().Column(c));

                    }
                    else
                    {
                        var oldSchemas = property.old.GetPropertySchemas();


                        foreach (var newSchema in property.actual.GetPropertySchemas())
                        {


                            var oldSchema = oldSchemas.FirstOrDefault(a => a.PlatformType.Guid == newSchema.PlatformType.Guid);


                            // Если поле уже было раньше и там лежит значение (приметивный тип) и они разные - меняем
                            if (oldSchema != null
                                && newSchema.SchemaType == XCColumnSchemaType.Value
                                && !oldSchema.PlatformType.Equals(newSchema.PlatformType))
                            {
                                query.Alter().Column(GetColumnDefenitionBySchema(newSchema));

                            }

                            if (oldSchema == null) // если раньше колонки небыло - создаем
                            {
                                query.Create().Column(GetColumnDefenitionBySchema(newSchema));
                            }

                        }



                    }
                }
            }

            return query;
        }
        
        public IExpression GetStep3(XCObjectTypeBase oldBase, XCObjectTypeBase actualBase)
        {
            XCSingleEntity old = (XCSingleEntity)oldBase;
            XCSingleEntity actual = (XCSingleEntity)actualBase;

            DDLQuery query = new DDLQuery();
            if (old != null && actual != null)
            {
                query.Delete().Table($"{actual.RelTableName}_tmp");
            }
            return query;
        }

        public IExpression GetStep4(XCObjectTypeBase oldBase, XCObjectTypeBase actualBase)
        {
            XCSingleEntity old = (XCSingleEntity)oldBase;
            XCSingleEntity actual = (XCSingleEntity)actualBase;

            DDLQuery query = new DDLQuery();
            if (old != null && actual != null)
            {
                query.Rename().Table($"{actual.RelTableName}_tmp").To(old.RelTableName);
            }
            return query;
        }


        
*/

/*

        public ColumnDefinition GetColumnDefenitionBySchema(XCColumnSchemaDefinition schema)
        { 

            ColumnDefinitionBuilder builder = new ColumnDefinitionBuilder();
            builder.WithColumnName(schema.Name);

            if (schema.SchemaType == XCColumnSchemaType.Value
                || schema.SchemaType == XCColumnSchemaType.Type)
            {

                var type = schema.PlatformType as XCPrimitiveType ??
                           throw new Exception("The value can be only primitive type");

                switch (type)
                {
                    case XCBoolean t:
                        builder.AsBoolean();
                        break;
                    case XCString t:
                        builder.AsString(t.Size);
                        break;
                    case XCDateTime t:
                        builder.AsDateTime();
                        break;
                    case XCGuid t:
                        builder.AsGuid();
                        break;
                    case XCNumeric t:
                        builder.AsFloat();
                        break;
                    case XCBinary t:
                        builder.AsBinary(t.Size);
                        break;
                    case XCInt t:
                        builder.AsInt32();
                        break;
                }
            }
            else if (schema.SchemaType == XCColumnSchemaType.Ref)
            {
                builder.AsGuid();
            }

            return builder.ColumnDefinition;
        }

        public List<ColumnDefinition> GetColumnDefenition(XCSingleEntityProperty property)
        {
            List<ColumnDefinition> columns = new List<ColumnDefinition>();
            

            property.DatabaseColumnName = $"fld{property.Id}";

            var columnSchemas = property.GetPropertySchemas(property.DatabaseColumnName);

            columnSchemas.ForEach(c => columns.Add(GetColumnDefenitionBySchema(c)));
 
            return columns;
        }
        
        */

    }
}