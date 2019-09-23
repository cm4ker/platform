

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to define the foreign key columns
    /// </summary>
    public interface ICreateForeignKeyForeignColumnSyntax 
    {
        /// <summary>
        /// Define the foreign key column
        /// </summary>
        /// <param name="column">The column name</param>
        /// <returns>Define the foreign keys primary table</returns>
        ICreateForeignKeyToTableSyntax ForeignColumn(string column);

        /// <summary>
        /// Define the foreign key columns
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns>Define the foreign keys primary table</returns>
        ICreateForeignKeyToTableSyntax ForeignColumns(params string[] columns);
    }
}
