

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the table of the foreign key
    /// </summary>
    public interface IDeleteForeignKeyOnTableSyntax 
    {
        /// <summary>
        /// Specify the table of the foreign key
        /// </summary>
        /// <param name="foreignTableName">The foreign keys table name</param>
        /// <returns>The next step</returns>
        IInSchemaSyntax OnTable(string foreignTableName);
    }
}
