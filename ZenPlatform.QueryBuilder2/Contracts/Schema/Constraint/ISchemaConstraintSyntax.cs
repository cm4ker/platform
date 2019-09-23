

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Query a constraints existence
    /// </summary>
    public interface ISchemaConstraintSyntax 
    {
        /// <summary>
        /// Returns <c>true</c> when a constraint exists
        /// </summary>
        /// <returns><c>true</c> when a constraint exists</returns>
        bool Exists();
    }
}
