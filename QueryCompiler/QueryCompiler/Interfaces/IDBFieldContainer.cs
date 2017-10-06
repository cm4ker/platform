using System.Collections.Generic;
using QueryCompiler.Queries;

namespace QueryCompiler.Interfaces
{
    public interface IDBFieldContainer : IDBToken
    {
        List<DBClause> Fields { get; }
        DBClause GetField(string name);
    }

    public interface IDBAliasedFieldContainer : IDBFieldContainer, IDbAliasedDbToken
    {

    }

    public interface IDBQueriableFieldContainer : IQueryable, IDBFieldContainer
    {
    }

}