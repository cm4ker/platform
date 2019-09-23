

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define table schema, a new column or set the tables description
    /// </summary>
    public interface ICreateTableWithColumnOrSchemaOrDescriptionSyntax : ICreateTableWithColumnOrSchemaSyntax
    {
        /// <summary>
        /// Set the tables description
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>Define the table schema or a new column</returns>
        ICreateTableWithColumnOrSchemaSyntax WithDescription(string description);
    }
}
