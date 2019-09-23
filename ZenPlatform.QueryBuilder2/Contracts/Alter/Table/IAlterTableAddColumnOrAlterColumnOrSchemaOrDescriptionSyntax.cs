

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to change the description or alter the table/column/schema
    /// </summary>
    public interface IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax : IAlterTableAddColumnOrAlterColumnOrSchemaSyntax
    {
        /// <summary>
        /// Set the description
        /// </summary>
        /// <param name="description">The description to set</param>
        /// <returns>Interface providing ways for other modifications</returns>
        IAlterTableAddColumnOrAlterColumnOrSchemaSyntax WithDescription(string description);
    }
}
