

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the foreign keys primary table
    /// </summary>
    public interface ICreateForeignKeyToTableSyntax 
    {
        /// <summary>
        /// Specify the foreign keys primary table
        /// </summary>
        /// <param name="table">The table name</param>
        /// <returns>Specify the primary tables columns</returns>
        ICreateForeignKeyPrimaryColumnOrInSchemaSyntax ToTable(string table);
    }
}
