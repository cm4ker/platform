using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.DataComponent.Interfaces
{
    public interface IQueryBuilder
    {
        IQueryable Build(PObjectType objectType);

    }
}
