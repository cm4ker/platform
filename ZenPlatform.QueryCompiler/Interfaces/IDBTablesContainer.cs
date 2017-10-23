using System.Collections.Generic;

namespace ZenPlatform.QueryCompiler.Interfaces
{
    public interface IDBTablesContainer
    {
        List<IDBTableDataSource> Tables { get; }
    }


}