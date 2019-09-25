

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to add or alter a column
    /// </summary>
    public interface IAlterTableAddColumnOrAlterColumnSyntax 
    {
        /// <summary>
        /// Add a column
        /// </summary>
        /// <param name="name">The column name</param>
        /// <returns>The interface to define the column properties</returns>
        IAlterTableColumnAsTypeSyntax AddColumn(string name);

        /// <summary>
        /// Alter a column
        /// </summary>
        /// <param name="name">The column name</param>
        /// <returns>The interface to define the column properties</returns>
        IAlterTableColumnAsTypeSyntax AlterColumn(string name);

        /// <summary>
        /// Set the table schema
        /// </summary>
        /// <param name="name">The schema name</param>
        void ToSchema(string name);
    }
}
