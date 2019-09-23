

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the new table name
    /// </summary>
    public interface IRenameTableToSyntax
    {
        /// <summary>
        /// Specify the new name of the table
        /// </summary>
        /// <param name="name">The new table name</param>
        /// <returns>The next step</returns>
        IInSchemaSyntax To(string name);
    }
}
