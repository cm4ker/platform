using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DataComponent.Interfaces;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Queries;
using DBType = ZenPlatform.QueryBuilder.Schema.DBType;
using IQueryable = ZenPlatform.QueryBuilder.Queries.IQueryable;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public class UpdateQueryBuilder : IQueryBuilder
    {
        public IQueryable Build(PObjectType objectType)
        {
            var query = DBQueryFactory.Instance.GetUpdate();

            var convertor = new ObjectToTableConvertor();

            query.UpdateTable = convertor.Convert(objectType);

            foreach (var field in query.UpdateTable.Fields.Where(f => !f.Schema.IsKey))
            {
                query.AddField(field as DBTableField);
            }


            var keyField = query.UpdateTable.Fields.First(p => p.Schema.IsKey);

            query.Where(keyField,CompareType.Equals, DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier));

            return query;
        }
    }
}
