

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Definition of the table the index belongs to
    /// </summary>
    public interface ICreateIndexForTableSyntax 
    {
        /// <summary>
        /// Defines the table the new index belongs to
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>Column or schema definition</returns>
        ICreateIndexOnColumnOrInSchemaSyntax OnTable(string tableName);
    }
}
