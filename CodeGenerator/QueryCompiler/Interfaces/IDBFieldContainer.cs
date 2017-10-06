using System.Collections.Generic;

namespace QueryCompiler
{
    public interface IDBFieldContainer : IDBAliasedToken
    {
        List<DBClause> Fields { get; }
        DBClause GetField(string name);
    }
}