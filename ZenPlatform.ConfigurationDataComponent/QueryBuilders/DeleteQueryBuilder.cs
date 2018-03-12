using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.DataComponent.Interfaces;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder.Schema;

using IQueryable = ZenPlatform.QueryBuilder.Queries.IQueryable;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public class DeleteQueryBuilder : IQueryBuilder
    {
        public IQueryable Build(PObjectType objectType)
        {
            var query = DBQueryFactory.Instance.GetDelete();

            var convertor = new ObjectToTableConvertor();

            query.DeleteTable = convertor.Convert(objectType);

            var keyField = query.DeleteTable.Fields.First(p => p.Schema.IsKey);

            query.Where(keyField, CompareType.Equals, DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier));

            return query;
        }
    }
}
