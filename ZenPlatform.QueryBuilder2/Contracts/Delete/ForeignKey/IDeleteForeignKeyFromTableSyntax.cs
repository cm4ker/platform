

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the table of the foreign key to delete
    /// </summary>
    public interface IDeleteForeignKeyFromTableSyntax 
    {
        /// <summary>
        /// Specify the table of the foreign key
        /// </summary>
        /// <param name="foreignTableName">The foreign key table name</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyForeignColumnOrInSchemaSyntax FromTable(string foreignTableName);
    }
}
