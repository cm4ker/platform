

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the table to create of foreign key for
    /// </summary>
    public interface ICreateForeignKeyFromTableSyntax 
    {
        /// <summary>
        /// Specify the table to create of foreign key for
        /// </summary>
        /// <param name="table">The table name</param>
        /// <returns>The schema or foreign key columns</returns>
        ICreateForeignKeyForeignColumnOrInSchemaSyntax FromTable(string table);
    }
}
