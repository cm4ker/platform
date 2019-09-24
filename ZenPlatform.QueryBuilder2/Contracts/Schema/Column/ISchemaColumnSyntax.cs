

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Query a columns existence
    /// </summary>
    public interface ISchemaColumnSyntax 
    {
        /// <summary>
        /// Returns <c>true</c> when the column exists
        /// </summary>
        /// <returns><c>true</c> when the column exists</returns>
        bool Exists();
    }
}
