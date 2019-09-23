

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the primary table for the constraint
    /// </summary>
    public interface ICreateConstraintOnTableSyntax 
    {
        /// <summary>
        /// Specify the primary table for the constraint
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>Define the schema or columns</returns>
        ICreateConstraintWithSchemaOrColumnSyntax OnTable(string tableName);
    }
}
