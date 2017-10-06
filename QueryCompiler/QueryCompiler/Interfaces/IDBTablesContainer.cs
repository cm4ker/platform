using System.Collections.Generic;
using QueryCompiler.Queries;

namespace QueryCompiler.Interfaces
{
    public interface IDBTablesContainer
    {
        List<IDBTableDataSource> Tables { get; }
    }


}