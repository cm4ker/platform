﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ZenPlatform.DataComponent.Interfaces;
using ZenPlatform.Configuration.Data;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public class SelectQueryBuilder: IQueryBuilder
    {

        public IDataChangeQuery Build(PObjectType objectType)
        {
            var query = DBQueryFactory.Instance.GetSelect();

            try
            { 
                var convertor = new ObjectToTableConvertor();

                DBTable table = convertor.Convert(objectType);

                var keyField = table.Fields.First(p => p.Schema.IsKey);


                var param = DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier);
                

                query.From(table)
                    .Where(keyField, CompareType.Equals, param);
                query.SelectAllFieldsFromSourceTables();

            } catch (InvalidOperationException ex)
            {
                throw new NotSupportedException("В таблице не может быть более одного уникального поля.", ex);
            }

            return query;
        }

    }
}
