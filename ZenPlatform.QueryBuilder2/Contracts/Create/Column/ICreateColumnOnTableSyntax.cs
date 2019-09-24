

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to specify the table for a column
    /// </summary>
    public interface ICreateColumnOnTableSyntax 
    {
        /// <summary>
        /// Specify the table for the column
        /// </summary>
        /// <param name="name">The table name</param>
        /// <returns>The interface to specify the table schema or column information</returns>
        ICreateColumnAsTypeOrInSchemaSyntax OnTable(string name);
    }
}
