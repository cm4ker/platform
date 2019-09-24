

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to specify the table for the column modification
    /// </summary>
    public interface IAlterColumnOnTableSyntax
    {
        /// <summary>
        /// Specify the table for the column modification
        /// </summary>
        /// <param name="name">The table name</param>
        /// <returns>Interface for the column modification or the schema specification</returns>
        IAlterColumnAsTypeOrInSchemaSyntax OnTable(string name);
    }
}
