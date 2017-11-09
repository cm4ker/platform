﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public class ObjectToTableConvertor
    {
        public DBTable Convert(PObjectType objectType)
        {
            DBTable table = new DBTable(objectType.TableName);
            foreach (var property in objectType.Properties)
            {
                if (property.Types.Count > 1)
                    throw new NotSupportedException("objectType не должен содержать свойства с несколькими типами.");

                var typeProperty = property.Types.FirstOrDefault() as PPrimetiveType;


                var schema = new DBFieldSchema(typeProperty.DBType, property.Name, typeProperty.ColumnSize,
                    typeProperty.Precision, typeProperty.Scale, false, property.Unique,
                    false, property.Unique ? false : typeProperty.IsNullable);

                var field = new DBTableField(table, property.Name)
                {
                    Schema = schema
                };

                table.Fields.Add(field);


            }

            return table;

        }
    }
}