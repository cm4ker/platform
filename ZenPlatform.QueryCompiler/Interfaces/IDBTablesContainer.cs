using System.Collections.Generic;

namespace ZenPlatform.QueryBuilder.Interfaces
{
    public interface IDBTablesContainer
    {
        List<IDBTableDataSource> Tables { get; }
    }


}