

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines a sequence and (optionally) the schema it's stored in
    /// </summary>
    public interface ICreateSequenceInSchemaSyntax : ICreateSequenceSyntax
    {
        /// <summary>
        /// Defines the schema of the sequence
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Defines the sequence options</returns>
        ICreateSequenceSyntax InSchema(string schemaName);
    }
}
