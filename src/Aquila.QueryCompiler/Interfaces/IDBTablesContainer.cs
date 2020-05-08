using System.Collections.Generic;

namespace Aquila.QueryBuilder.Interfaces
{
    public interface IDBTablesContainer
    {
        List<IDBDataSource> Tables { get; }
    }


}