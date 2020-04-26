using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.QueryBuilder.Interfaces
{
    public interface IDBFieldContainer : IDBToken
    {
        List<DBField> Fields { get; }
        DBField GetField(string name);
    }

    public interface IDBAliasedFieldContainer : IDBFieldContainer, IDbAliasedDbToken
    {

    }

    public interface IDBQueriableFieldContainer : IQueryable, IDBFieldContainer
    {
    }

}