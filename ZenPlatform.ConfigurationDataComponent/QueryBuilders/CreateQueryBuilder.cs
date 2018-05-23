using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.DataComponent.Interfaces;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Queries;
using IQueryable = ZenPlatform.QueryBuilder.Queries.IQueryable;

namespace ZenPlatform.DataComponent.QueryBuilders
{
    public class CreateQueryBuilder : IQueryBuilder
    {
        public IQueryable Build(PObjectType objectType)
        {
            var query = DBQueryFactory.Instance.GetInsert();

            var convertor = new ObjectToTableConvertor();

            query.InsertTable = convertor.Convert(objectType);

            foreach (var field in query.InsertTable.Fields.Where(f => !f.Schema.IsKey))
            {
                query.AddField(field as DBTableField);
            }

            return query;
        }

    }
}
