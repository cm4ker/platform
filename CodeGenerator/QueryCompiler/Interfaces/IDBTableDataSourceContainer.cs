using System.Collections.Generic;

namespace QueryCompiler
{
    public interface IDBTableDataSourceContainer : IQueryable
    {
        List<IDBTableDataSorce> Tables { get; }
        IDBTableDataSorce GetTable(string alias);
    }
}