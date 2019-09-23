

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the target column name
    /// </summary>
    public interface IRenameColumnToSyntax 
    {
        /// <summary>
        /// Specify the new column name
        /// </summary>
        /// <param name="name">The new column name</param>
        void To(string name);
    }
}
