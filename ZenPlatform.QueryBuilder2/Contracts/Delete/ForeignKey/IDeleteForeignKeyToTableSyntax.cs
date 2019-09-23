

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the primary table of the foreign key
    /// </summary>
    public interface IDeleteForeignKeyToTableSyntax 
    {
        /// <summary>
        /// Specify the primary table of the foreign key
        /// </summary>
        /// <param name="table">The primary table name</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyPrimaryColumnSyntax ToTable(string table);
    }
}
